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
    public class CustomerController : Controller
    {
        CustomerServices CustomerServices;
        private readonly IMapper _mapper;

        public CustomerController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            CustomerServices = new CustomerServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetCustomerlist()
        {
            var result = await CustomerServices.getCustomerList();

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var result = await CustomerServices.getCustomer(id);
            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutCustomer(int id, [FromBody] List<CustomerDTO> customerupdated)
        {
            var result = await CustomerServices.updateCustomer(id, customerupdated);
            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCustomer([FromBody] List<CustomerDTO> customer)
        {
            var result = await CustomerServices.addCustomer(customer);
            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await CustomerServices.deleteCustomer(id);
            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

