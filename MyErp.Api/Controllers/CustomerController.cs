using AutoMapper;
using License.Core.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;
//using Type = MyErp.Core.Models.Type;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class CustomerController : Controller
    {
        CustomerServices CustomerServices;
        private readonly IMapper _mapper;
        private readonly RightsModelServices _accessService;

        public CustomerController(ApplicationDbContext dBContext, IMapper mapper, RightsModelServices accessService)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            CustomerServices = new CustomerServices(unitOfWork, _mapper);
            _accessService = accessService;
        }
        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await CustomerServices.deleteAll();
            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
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
        [HttpGet("getAllowedCustomer")]
        public async Task<IActionResult> GetAllowedCustomer()
        {
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);
            var result = await CustomerServices.getProjectsByAccess(currentUser);
            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpGet("GetAllOdoo")]
        public async Task<IActionResult> GetCustomerlistOdoo()
        {

            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var result = await CustomerServices.getCustomersOdoo(usertype);

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;

        }
        [Authorize]
        [HttpGet("GetAllByType")]
        public async Task<IActionResult> GetCustomerlist()
        {

            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var result = await CustomerServices.getCustomersListByType( usertype);

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
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var result = await CustomerServices.updateCustomer(id, customerUpdated , currentUser);

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> putCustomer([FromBody] List<CustomerDTO> customerUpdated)
        {
            var currentuser = User.Identity?.Name;
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);
            var result = await CustomerServices.addCustomer(customerUpdated , usertype, currentUser);

            var resultWithStatusCode = ResponseStatusCode<Customer>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        [HttpPost("addFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var currentuser = User.Identity?.Name;

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
