using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly GetUSerId _getUSerId;

        public NotificationController(NotificationService notificationService, GetUSerId getUSerId)
        {
            _notificationService = notificationService;
            _getUSerId = getUSerId;
        }

        // ================= GET ALL =================
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = User.Identity?.Name;
            string AssignedId =await _getUSerId.GetUserIdByUsernameAsync(currentUser);
            var result = await _notificationService.GetAll(AssignedId);

            return Ok(result);
        }
        //[HttpDelete("deleteAll")]
        //public async Task<IActionResult> DeleteAll()
        //{
        //    var result = await NotificationService.deleteAll();
        //    var resultWithStatusCode = ResponseStatusCode<CalenderTask>.GetApiResponseCode(result, "HttpDelete");

        //    return resultWithStatusCode;
        //}
        // ================= MARK AS READ =================
        [HttpPost("markAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsRead(id);

            if (result.errors.Any())
                return BadRequest(result);

            return Ok(result);
        }

        // ================= MARK ALL AS READ =================
        [HttpPost("markAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var currentUser = User.Identity?.Name;
           
            string AssignedId = await _getUSerId.GetUserIdByUsernameAsync(currentUser);

            var result = await _notificationService.MarkAllAsRead(AssignedId);

            return Ok(result);
        }

        // ================= GET UNREAD COUNT =================
        //[HttpGet("unreadCount")]
        //public async Task<IActionResult> GetUnreadCount()
        //{
        //    var currentUser = User.Identity?.Name;

        //    var result = await _notificationService.GetUnreadCount(currentUser);

        //    return Ok(result);
        //}

        // ================= SEND (OPTIONAL TEST) =================
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification(string userId, string message)
        {
            var sender = User.Identity?.Name;

            var result = await _notificationService.AddNotificationAsync(userId, message, sender);

            return Ok(result);
        }
    }
}