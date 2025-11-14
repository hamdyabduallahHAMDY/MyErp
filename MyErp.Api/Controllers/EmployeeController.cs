using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.Core.Validation;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class EmployeeController : Controller
    {
        EmployeeServices EmployeeServices;
        private readonly IMapper _mapper;

        public EmployeeController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            EmployeeServices = new EmployeeServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetEmployeelist()
        {
            var result = await EmployeeServices.getEmployeeList();

            var resultWithStatusCode = ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var result = await EmployeeServices.getEmployee(id);
            var resultWithStatusCode = ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] List<EmployeeDTO> employeeupdated)
        {
            var result = await EmployeeServices.updateEmployee(id, employeeupdated);
            var resultWithStatusCode = ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] List<EmployeeDTO> employee)
        {
            var result = await EmployeeServices.addEmployee(employee);
            var resultWithStatusCode = ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await EmployeeServices.deleteEmployee(id);
            var resultWithStatusCode = ResponseStatusCode<Employee>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

