using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;
using System.Text.Json;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class CalenderTaskController : Controller
    {
        CalendarTaskServices CalendarTasksServices;
        private readonly IMapper _mapper;
        private readonly GetUSerId _getUSerId;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly NotificationService _notificationService;
        public CalenderTaskController(ApplicationDbContext dBContext, IMapper mapper, GetUSerId getUSerId, IHubContext<NotificationHub> hub, NotificationService notificationService)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            CalendarTasksServices = new CalendarTaskServices(unitOfWork, _mapper, hub, notificationService);
            _getUSerId = getUSerId;

        }
        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await CalendarTasksServices.deleteAll();
            var resultWithStatusCode = ResponseStatusCode<CalenderTask>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetCalendarTasksList()
        {
            var result = await CalendarTasksServices.getCalendarTasksList();

            var resultWithStatusCode = ResponseStatusCode<CalenderTask>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetCalendarTask(int id)
        {
            var result = await CalendarTasksServices.getCalendarTask(id);

            var resultWithStatusCode = ResponseStatusCode<CalenderTask>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        [HttpPut("updateById")]
        public async Task<IActionResult> PutCalendarTask(int id, CalenderTaskDTO taskUpdated)
        {
            var result = await CalendarTasksServices.updateCalendarTask(id, taskUpdated);

            var resultWithStatusCode = ResponseStatusCode<CalenderTask>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddCalendarTask([FromBody] CalenderTaskDTO task)
        {
            var createdby = User.Identity.Name;

            // 🔥 Deserialize usernames
            var assignedUsernames = JsonSerializer.Deserialize<List<string>>(task.AssignedTo);

            // 🔥 Convert usernames → userIds
            var assignedIds = new List<string>();

            foreach (var username in assignedUsernames)
            {
                var userId = await _getUSerId.GetUserIdByUsernameAsync(username);
                if (!string.IsNullOrEmpty(userId))
                    assignedIds.Add(userId);
            }

            var result = await CalendarTasksServices.addCalendarTask(task, createdby, assignedIds);

            var resultWithStatusCode =
                ResponseStatusCode<CalenderTask>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteCalendarTask(int id)
        {
            var result = await CalendarTasksServices.deleteCalendarTask(id);

            var resultWithStatusCode = ResponseStatusCode<CalenderTask>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}