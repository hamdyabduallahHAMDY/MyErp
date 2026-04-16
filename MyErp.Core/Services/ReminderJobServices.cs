using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using System.Text.Json;

namespace MyErp.Core.Services
{
    public class ReminderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderWorker> _logger;

        public ReminderWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<ReminderWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReminderWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                    var now = DateTime.UtcNow;

                    var tasks = await unitOfWork.CalenderTasks.GetAll(t =>
                        t.ReminderTime <= now && t.IsReminderSent == false);

                    _logger.LogInformation("Found {Count} tasks to process", tasks.Count());

                    foreach (var task in tasks)
                    {
                        // 🔥 Parse assigned users (JSON or fallback)
                        List<string> assignedUsers = new();

                        if (!string.IsNullOrEmpty(task.AssignedTo))
                        {
                            try
                            {
                                assignedUsers = JsonSerializer.Deserialize<List<string>>(task.AssignedTo)
                                                ?? new List<string>();
                            }
                            catch
                            {
                                assignedUsers.Add(task.AssignedTo);
                            }
                        }

                        foreach (var username in assignedUsers)
                        {
                            var user = await userManager.FindByNameAsync(username);

                            if (user == null)
                            {
                                _logger.LogWarning("User not found: {Username}", username);
                                continue;
                            }

                            // 🔥 Save notification
                            var notification = new Notification
                            {
                                UserId = user.Id,
                                title = "Reminder for Calendar Task!",
                                Message = task.Title,
                                CreatedAt = DateTime.UtcNow,
                                IsRead = false,
                                CreatedBy = task.CreatedBy
                            };

                            await unitOfWork.Notifications.Add(notification);

                            // 🔥 Send SignalR notification
                            await hub.Clients.User(user.Id)
                                .SendAsync("ReceiveNotification", new
                                {
                                    title = "Reminder for Calendar Task!",
                                    taskTitle = task.Title,
                                    assignedBy = task.CreatedBy,
                                    message = $"\"{task.Title}\" is due soon",
                                    type = "CalendarTaskReminder",
                                    createdAt = DateTime.UtcNow
                                });

                            _logger.LogInformation("Reminder sent for task {Task} to {User}",
                                task.Title, username);
                        }

                        // 🔥 Mark task as processed AFTER all users handled
                        task.IsReminderSent = true;
                    }

                    await unitOfWork.Complete();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in ReminderWorker");
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }

            _logger.LogInformation("ReminderWorker stopped.");
        }
    }
}