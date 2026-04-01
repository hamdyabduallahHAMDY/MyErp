using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [HttpGet("template/customer")]
        public async Task<IActionResult> DownloadCustomerTemplate()
        {

            var fileBytes = await CustomerServices.GenerateCustomerExcelTemplate();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Customer_Template.xlsx"
            );
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetCustomerlist()
        {
            var result = await CustomerServices.getCustomersList();

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
        public async Task<IActionResult> putCustomer(int id, List<CustomerDTO> customerUpdated)
        {
            var result = await CustomerServices.updateCustomer(id, customerUpdated);

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> putCustomer([FromBody] List<CustomerDTO> customerUpdated)
        {
            var currentuser = User.Identity.Name;
            var result = await CustomerServices.addCustomer(customerUpdated);

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        [HttpPost("addFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var currentuser = User.Identity.Name;

            var result = await CustomerServices.ImportFromExcel(file , currentuser);
            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }

        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteCust(int id)
        {
            var result = await CustomerServices.deleteUser(id);

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
        [HttpDelete("deleteGroupById")]
        public async Task<IActionResult> DeleteGroupCust(List<int> id)
        {
            var result = await CustomerServices.deleteGroup(id);

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}
