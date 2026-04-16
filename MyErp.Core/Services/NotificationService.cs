using AutoMapper;
using Logger;
using Microsoft.AspNetCore.SignalR;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;

namespace MyErp.Core.Services
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHubContext<NotificationHub> hub)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hub = hub;
        }

        // ================= ADD =================
        public async Task<MainResponse<Notification>> AddNotificationAsync(
            string receiverId,
            string message,
            string sender = null)
        {
            var response = new MainResponse<Notification>();

            try
            {
                var notification = new Notification
                {
                    UserId = receiverId,
                    Message = message,
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };

                await _unitOfWork.Notifications.Add(notification);

                response.acceptedObjects?.Add(notification);

                // Send Real-Time Notification  
                await _hub.Clients.User(receiverId)
                    .SendAsync("ReceiveNotification", message);

            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }

        // ================= GET ALL =================
            public async Task<MainResponse<Notification>> GetAll(string currentUser )
            {

               var response = new MainResponse<Notification>();

            try
            {
                var notifications = await _unitOfWork.Notifications.GetAll(a => a.UserId == currentUser);
                if (string.IsNullOrEmpty(currentUser))
                {
                    response.acceptedObjects = new List<Notification>();
                    return response;
                }

                if (notifications == null || !notifications.Any())
                {
                    response.errors?.Add("No notifications found.");
                    return response;
                }
                response.acceptedObjects = notifications
                    .Reverse()
                    .ToList();
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response?.errors?.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }

        public async Task<MainResponse<Notification>> MarkAsRead(int id)
        {
            var response = new MainResponse<Notification>();

            try
            {
                var notif = await _unitOfWork.Notifications.GetById(id);

                if (notif == null)
                {
                    response.errors.Add("Notification not found");
                    return response;
                }

                notif.IsRead = true;

                await _unitOfWork.Notifications.Update(notif);

                response.acceptedObjects.Add(notif);
            }
            catch (Exception ex)
            {
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<Notification>> MarkAllAsRead(string userId)
        {
            var response = new MainResponse<Notification>();

            try
            {
                var uselessVariable = "MOSallaaaaaaaaaaaaaaaaaaaaah right on the wing mosalah mosalah running on the wiiiiiiing ";
                var notifications = await _unitOfWork.Notifications
                    .GetAll(n => n.UserId == userId && !n.IsRead);

                foreach (var notif in notifications)
                {
                    notif.IsRead = true;
                    await _unitOfWork.Notifications.Update(notif);
                }

                response.acceptedObjects = notifications.ToList();
            }
            catch (Exception ex)
            {
                response.errors?.Add(ex.Message);
            }

            return response;
        }
    }
}