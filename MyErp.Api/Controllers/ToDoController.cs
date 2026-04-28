using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using Type = MyErp.Core.Models.Type;

namespace MyErp.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("ToDo")]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoServices _toDoServices;
        private readonly RightsModelServices _accessService;
        private readonly GetUSerId _getUserId;
        private readonly IMapper _mapper;

        public ToDoController(
            ToDoServices toDoServices,
            RightsModelServices accessService,
            GetUSerId getUserId,
            IMapper mapper)
        {
            _toDoServices = toDoServices;
            _accessService = accessService;
            _getUserId = getUserId;
            _mapper = mapper;
        }

        [HttpGet("template/todo")]
        public async Task<IActionResult> DownloadToDoTemplate()
        {
            var fileBytes = await _toDoServices.GenerateToDoExcelTemplate();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ToDo_Template.xlsx");
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            Logger.Logs.Log($"User : [{currentUser}] requesting all ToDos");

            var result = await _toDoServices.GetAll(allowedUsers);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetById(int id)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.GetById(id, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpGet("getByStatus")]
        public async Task<IActionResult> GetByStatus(int status)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.GetByStatus(status, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpGet("GetAllBycCustomer")]
        public async Task<IActionResult> GetAllByCustomerForOdoo([FromQuery] string customer)
        {
            var (currentUser, allowedUsers, isAuth, userType) = _accessService.GetAccessData(User);

            var result = await _toDoServices.GetAllByOdooCustomerType(userType, customer);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpGet("Get_By_type")]
        public async Task<IActionResult> GetByType()
        {
            var (currentUser, allowedUsers, isAuth, userType) = _accessService.GetAccessData(User);

            var result = await _toDoServices.GetByType(userType, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpGet("Get by Assgined To")]
        public async Task<IActionResult> GetByAssignedTo([FromQuery] string assignedTo)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.GetByAssignedTo(assignedTo, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpGet("Get_By_Customer")]
        public async Task<IActionResult> GetByCustomer([FromQuery] string customer)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.GetByCustomer(customer, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpGet("Get_By_Customer_Type")]
        public async Task<IActionResult> GetByCustomerType([FromQuery] string customer)
        {
            var (currentUser, allowedUsers, isAuth, userType) = _accessService.GetAccessData(User);

            var result = await _toDoServices.GetByCustomerAndType(customer, userType, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] ToDoDTO dto)
        {
            var (currentUser, allowedUsers, isAuth, userType) = _accessService.GetAccessData(User);

            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return Unauthorized(new { message = "You must log in first." });
            }

            var assignedUserId = await _getUserId.GetUserIdByUsernameAsync(dto?.AssignedTo);

            var result = await _toDoServices.AddToDo(dto, currentUser, userType, assignedUserId);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPost");
        }

        [HttpPost("addFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var (currentUser, allowedUsers, isAuth, userType) = _accessService.GetAccessData(User);

            if (string.IsNullOrWhiteSpace(currentUser))
            {
                return Unauthorized(new { message = "You must log in first." });
            }

            var result = await _toDoServices.ImportFromExcel(file, currentUser, userType);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPost");
        }

        [HttpPut("updateById")]
        public async Task<IActionResult> Update(int id, [FromBody] ToDoDTO dto)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.UpdateToDo(id, dto, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPut");
        }

        [HttpPut("UpadteStatus")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] int status)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.UpdateStatus(id, status, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPut");
        }

        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.DeleteToDo(id, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpDelete");
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAllMine()
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.DeleteAll(currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpDelete");
        }

        [HttpDelete("deleteGroup")]
        public async Task<IActionResult> DeleteGroup([FromBody] List<int> ids)
        {
            var currentUser = User.Identity?.Name;
            var result = await _toDoServices.DeleteGroup(ids, currentUser);
            return ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpDelete");
        }
    }
}