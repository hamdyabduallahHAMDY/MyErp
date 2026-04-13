using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;
using Mysqlx;
using System.Text.Json;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class ToDoController : Controller
    {
        ToDoServices ToDoServices;
        private readonly IMapper _mapper;
        private readonly RightsModelServices _accessService;
        private readonly GetUSerId _getUSerId;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly NotificationService _notificationService;

        public ToDoController(
     ApplicationDbContext dbContext,
     IMapper mapper,
     RightsModelServices accessService,
     IHubContext<NotificationHub> hub,
     NotificationService notificationService,
     GetUSerId getUSerId

 )
        {
            hub = hub; 
            UnitOfWork unitOfWork = new UnitOfWork(dbContext);
            _mapper = mapper; _hub = hub;
            ToDoServices = new ToDoServices(unitOfWork, _mapper, hub, _notificationService); _accessService = accessService;
            _notificationService = notificationService;
            _getUSerId = getUSerId;
        }

        [HttpGet("template/todo")]
        public async Task<IActionResult> DownloadToDoTemplate()
        {
            var fileBytes = await ToDoServices.GenerateToDoExcelTemplate();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ToDo_Template.xlsx"
            );
        }
        //[HttpDelete("deleteAll")]
        //public async Task<IActionResult> DeleteAll()
        //{
        //    var result = await ToDoServices.deleteAll();
        //    var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpDelete");

        //    return resultWithStatusCode;
        //}
        [HttpGet("GetAllBycCustomer")]
        public async Task<IActionResult> GetAllByCustomer(string customer)
        {
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var result = await ToDoServices.GetAllByOdooCustomerType(usertype,customer);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetToDoList()
        {
            var currentUser = User.Identity?.Name;

            var result = await ToDoServices.GetAll(currentUser);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetToDo(int id)
        {
            var result = await ToDoServices.getToDo(id);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpGet("getByStatus")]
        public async Task<IActionResult> GetbyStatus(int status)
        {
            var currentUser = User.Identity?.Name;

            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    message = "You must log in first."
                });
            }

            var rightsJson = User.Claims.FirstOrDefault(c => c.Type == "Rights")?.Value;
            List<string> allowedUsers = new List<string>();

            if (!string.IsNullOrEmpty(rightsJson))
            {
                var rights = JsonSerializer.Deserialize<RightsModel>(rightsJson);

                if (rights?.allowance != null)
                    allowedUsers = rights.allowance;
            }

            // add current user
            allowedUsers.Add(currentUser);

            var result = await ToDoServices.getByStatus(status, allowedUsers);

            var resultWithStatusCode =
                ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [Authorize]
        [HttpPut("updateById")]
        public async Task<IActionResult> PutToDo(int id, [FromBody] ToDoDTO todoUpdated)
        {
            var currentUser = User.Identity?.Name;

            var result = await ToDoServices.updateToDo(id, todoUpdated, currentUser);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }

        [HttpPut("UpadteStatus")]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            var result = await ToDoServices.UpdateStatus(id, status);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToDo([FromBody] ToDoDTO todos)
        {
            //var currentUser = User.Identity?.Name;
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);
            var AssginedId = await _getUSerId.GetUserIdByUsernameAsync(todos?.AssignedTo);
            var result = await ToDoServices.addToDo(todos, currentUser, usertype ,AssginedId );
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }

        [HttpPost("addFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var result = await ToDoServices.ImportFromExcel(file);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }

        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            var result = await ToDoServices.deleteToDo(id);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpDelete");
            return resultWithStatusCode;
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await ToDoServices.deleteAll();
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpDelete");
            return resultWithStatusCode;
        }

        [HttpDelete("deleteGroup")]
        public async Task<IActionResult> DeleteGroup(List<int> ints)
        {
            var result = await ToDoServices.deleteGroup(ints);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpDelete");
            return resultWithStatusCode;
        }

        [HttpGet("Get by Assgined To")]
        public async Task<IActionResult> GetbyAssignedTo(string assginedto)
        {
            var result = await ToDoServices.GetByAssignedTo(assginedto);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpGet("Get_By_type")]
        public async Task<IActionResult> GetByType(int type)
        {
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var result = await ToDoServices.getByType(usertype);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }
        [HttpGet("Get_By_Customer")]
        public async Task<IActionResult> GetByCustomer(string customer)
        {
        //    var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var result = await ToDoServices.getByCustomer(customer);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }
        [HttpGet("Get_By_Customer_Type")]
        public async Task<IActionResult> GetByCustomerType(string customer)
        {
               var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var result = await ToDoServices.getByCustomerandType(customer,usertype);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

    }
}