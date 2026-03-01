using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class ToDoController : Controller
    {
        ToDoServices ToDoServices;
        private readonly IMapper _mapper;

        public ToDoController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            ToDoServices = new ToDoServices(unitOfWork, _mapper);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetToDoList()
        {
            var result = await ToDoServices.getToDoList();
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
            var result = await ToDoServices.getByStatus(status);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpPut("updateById")]
        public async Task<IActionResult> PutToDo(int id, [FromBody] List<ToDoDTO> todoUpdated)
        {
            var result = await ToDoServices.updateToDo(id, todoUpdated);
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
        public async Task<IActionResult> AddToDo([FromBody] List<ToDoDTO> todos)
        {
            var result = await ToDoServices.addToDo(todos);
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

        // OPTIONAL (recommended): toggle check/uncheck for daily tasks
        // Expects a boolean: true = check now, false = uncheck
        [HttpPut("toggleCheckById")]
        public async Task<IActionResult> ToggleCheck(int id, [FromQuery] bool check)
        {
            var result = await ToDoServices.toggleCheck(id, check);
            var resultWithStatusCode = ResponseStatusCode<ToDo>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
    }
}