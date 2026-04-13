using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class EmployeeController : Controller
    {
        private readonly EmployeeServices _employeeServices;
        private readonly IMapper _mapper;

        public EmployeeController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            _employeeServices = new EmployeeServices(unitOfWork, _mapper);
        }
        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await _employeeServices.deleteAll();
            var resultWithStatusCode = ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
        // ================= GET ALL =================
        [HttpGet("getAll")]
        public async Task<IActionResult> GetEmployeeList()
        {
            var result = await _employeeServices.getEmployeesList();

            var resultWithStatusCode =
                ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // ================= GET BY ID =================
        [HttpGet("getById")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var result = await _employeeServices.getEmployee(id);

            var resultWithStatusCode =
                ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // ================= UPDATE =================
        [HttpPut("updateById")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDTO employeeUpdated)
        {
            var result = await _employeeServices.updateEmployee(id, employeeUpdated);

            var resultWithStatusCode =
                ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }

        // ================= ADD =================
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDTO employees)
        {
         //   var currentuser = User.Identity?.Name;

            var result = await _employeeServices.AddEmployee(employees);

            var resultWithStatusCode =
                ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        // ================= DELETE ONE =================
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeServices.deleteEmployee(id);

            var resultWithStatusCode =
                ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }

        // ================= DELETE GROUP =================
        [HttpDelete("deleteGroupById")]
        public async Task<IActionResult> DeleteGroupEmployee([FromBody] List<int> ids)
        {
            var result = await _employeeServices.deleteGroup(ids);

            var resultWithStatusCode =
                ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}