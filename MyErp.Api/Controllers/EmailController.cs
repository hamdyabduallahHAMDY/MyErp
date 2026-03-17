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
    public class EmailController : Controller
    {
        EmailServices EmailServices;
        private readonly IMapper _mapper;

        public EmailController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;

            EmailServices = new EmailServices(unitOfWork, _mapper);
        }

        // GET ALL EMAILS
        [HttpGet("getAll")]
        public async Task<IActionResult> GetEmailList()
        {
            var result = await EmailServices.getEmailsList();

            var resultWithStatusCode =
                ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // GET EMAIL BY ID
        [HttpGet("getById")]
        public async Task<IActionResult> GetEmail(int id)
        {
            var result = await EmailServices.getEmail(id);

            var resultWithStatusCode =
                ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // UPDATE EMAIL
        [HttpPut("updateById")]
        public async Task<IActionResult> PutEmail(int id, [FromForm] EmailDTO emailUpdated)
        {
            var result = await EmailServices.updateEmail(id, emailUpdated);

            var resultWithStatusCode =
                ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }

        
        // ADD EMAIL
        [HttpPost("add")]
        public async Task<IActionResult> AddEmail([FromForm] EmailDTO emailDTOs)
        {
            var result = await EmailServices.addEmail(emailDTOs);

            var resultWithStatusCode =
                ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        // ADD EMAILS FROM EXCEL
        [HttpPost("addFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var result = await EmailServices.ImportFromExcel(file);

            var resultWithStatusCode =
                ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        // DELETE EMAIL
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteEmail(int id)
        {
            var result = await EmailServices.deleteEmail(id);

            var resultWithStatusCode =
                ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}