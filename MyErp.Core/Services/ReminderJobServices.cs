using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;

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

                    var now = DateTime.Now;
                    
                    var tasks = await unitOfWork.CalenderTasks.GetAll(t =>
                        t.ReminderTime <= now && t.IsReminderSent == false );

                    _logger.LogInformation($"Found {tasks.Count()} tasks to process");

                    foreach (var task in tasks)
                    {
                        var user = await userManager.FindByNameAsync(task.CreatedBy);
                      //  task.IsReminderSent = true;
                      //  await unitOfWork.CalenderTasks.Update(task);
                        if (user == null)
                        {
                            _logger.LogWarning($"User not found for task {task.Title}");
                            continue;
                        }

                        var notification = new Notification
                        {
                            UserId = user.Id,
                            Message = task.Title,
                            CreatedAt = DateTime.UtcNow,
                            IsRead = false,
                            CreatedBy = task.CreatedBy
                        };

                        await unitOfWork.Notifications.Add(notification);
                        _logger.LogCritical("sentNotif");
                        await hub.Clients.User(user.Id)
                            .SendAsync("ReceiveNotification", $"Reminder for a CalenderTask '{task.Title}'");

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