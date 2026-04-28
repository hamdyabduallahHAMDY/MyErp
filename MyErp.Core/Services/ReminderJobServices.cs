using Logger;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using System.Text.Json;

namespace MyErp.Core.Services
{
    public class ReminderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // How often the worker wakes up
        private static readonly TimeSpan WorkerInterval = TimeSpan.FromMinutes(5);

        // Max due tasks processed per cycle
        private const int BatchSize = 100;

        public ReminderWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.Logs.Log("[SYSTEM] ReminderWorker started.", null);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessReminderBatch(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // graceful shutdown
                    break;
                }
                catch (Exception ex)
                {
                    Logs.Log("[SYSTEM] Critical error in ReminderWorker loop.", ex);
                }

                try
                {
                    await Task.Delay(WorkerInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            Logs.Log("[SYSTEM] ReminderWorker stopped.", null);
        }

        private async Task ProcessReminderBatch(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            var now = DateTime.UtcNow;

            Logs.Log($"[SYSTEM] Reminder cycle started at {now:O}", null);

            // IMPORTANT:
            // Query only a small batch instead of loading all due tasks.
            var dueTasks = await unitOfWork.CalenderTasks
                .GetQueryable()
                .Where(t => t.ReminderTime <= now && !t.IsReminderSent)
                .OrderBy(t => t.ReminderTime)
                .Take(BatchSize)
                .ToListAsync(stoppingToken);

            if (dueTasks == null || dueTasks.Count == 0)
            {
                Logs.Log("[SYSTEM] No due reminder tasks found.", null);
                return;
            }

            Logs.Log($"[SYSTEM] Found {dueTasks.Count} due tasks to process.", null);

            // Build task -> usernames map once
            var taskAssignments = new Dictionary<CalenderTask, List<string>>();
            var allUsernames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var task in dueTasks)
            {
                var usernames = ParseAssignedUsers(task.AssignedTo);

                taskAssignments[task] = usernames;

                foreach (var username in usernames)
                {
                    if (!string.IsNullOrWhiteSpace(username))
                        allUsernames.Add(username.Trim());
                }
            }

            // Load all needed users in ONE query instead of FindByNameAsync inside loops
            var users = await userManager.Users
                .Where(u => u.UserName != null && allUsernames.Contains(u.UserName))
                .ToListAsync(stoppingToken);

            var userByUsername = users
                .Where(u => !string.IsNullOrWhiteSpace(u.UserName))
                .GroupBy(u => u.UserName!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var notificationsToInsert = new List<Notification>();
            var signalRSendTasks = new List<Task>();

            foreach (var task in dueTasks)
            {
                stoppingToken.ThrowIfCancellationRequested();

                var assignedUsers = taskAssignments[task];

                if (assignedUsers.Count == 0)
                {
                    Logs.Log($"[SYSTEM] Task '{task.Title}' has no assigned users. Marking as sent.", null);
                    task.IsReminderSent = true;
                    continue;
                }

                foreach (var username in assignedUsers)
                {
                    if (!userByUsername.TryGetValue(username, out var user))
                    {
                        Logs.Log($"[SYSTEM] User not found: {username}", null);
                        continue;
                    }

                    var notification = new Notification
                    {
                        UserId = user.Id,
                        title = "Reminder for Calendar Task!",
                        Message = task.Title,
                        CreatedAt = DateTime.UtcNow,
                        IsRead = false,
                        CreatedBy = task.CreatedBy
                    };

                    notificationsToInsert.Add(notification);

                    // Queue SignalR send task without blocking the whole loop more than needed
                    signalRSendTasks.Add(
                        hub.Clients.User(user.Id).SendAsync("ReceiveNotification", new
                        {
                            title = "Reminder for Calendar Task!",
                            taskTitle = task.Title,
                            assignedBy = task.CreatedBy,
                            message = $"\"{task.Title}\" is due soon",
                            type = "CalendarTaskReminder",
                            createdAt = DateTime.UtcNow
                        }, stoppingToken)
                    );
                }

                // mark task as processed after preparing all outputs
                task.IsReminderSent = true;
            }

            // Bulk add notifications if your repository supports List<T>
            if (notificationsToInsert.Count > 0)
            {
                await unitOfWork.Notifications.Add(notificationsToInsert);
            }

            // Save DB changes first
            await unitOfWork.Complete();

            // Then send SignalR messages
            if (signalRSendTasks.Count > 0)
            {
                try
                {
                    await Task.WhenAll(signalRSendTasks);
                }
                catch (Exception ex)
                {
                    Logs.Log("[SYSTEM] Error while sending one or more SignalR notifications.", ex);
                }
            }

            Logs.Log(
                $"[SYSTEM] Reminder cycle completed. Tasks processed: {dueTasks.Count}, Notifications created: {notificationsToInsert.Count}",
                null
            );
        }

        private static List<string> ParseAssignedUsers(string? assignedTo)
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(assignedTo))
                return result;

            try
            {
                // First try JSON array: ["user1","user2"]
                var parsed = JsonSerializer.Deserialize<List<string>>(assignedTo);
                if (parsed != null)
                {
                    result.AddRange(
                        parsed
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => x.Trim())
                    );
                }
            }
            catch
            {
                // fallback: single username stored as plain text
                result.Add(assignedTo.Trim());
            }

            return result
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}