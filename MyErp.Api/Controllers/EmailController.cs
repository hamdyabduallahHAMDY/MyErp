using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize]
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
            var currentUser = User.Identity?.Name;
           
            
            var rightsJson = User.Claims.FirstOrDefault(c => c.Type == "Rights")?.Value;
            List<string> allowedUsers = new List<string>();

            if (!string.IsNullOrEmpty(rightsJson))
            {
                var rights = JsonSerializer.Deserialize<RightsModel>(rightsJson);

                if (rights?.allowance != null)
                    allowedUsers = rights.allowance;
            }

            // 3. Add current user to allowed list
            allowedUsers.Add(currentUser);




            var result = await EmailServices.getEmailsList(allowedUsers);

            var resultWithStatusCode =
                ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("get by status")]
        public async Task<IActionResult> getbystatus(int status)
        {
            var currentUser = User.Identity?.Name;


            var rightsJson = User.Claims.FirstOrDefault(c => c.Type == "allowance")?.Value;
            List<string> allowedUsers = new List<string>();

            if (!string.IsNullOrEmpty(rightsJson))
            {
                var rights = JsonSerializer.Deserialize<RightsModel>(rightsJson);

                if (rights?.allowance != null)
                    allowedUsers = rights.allowance;
            }

            // 3. Add current user to allowed list
            allowedUsers.Add(currentUser);




            var result = await EmailServices.getByStatus(allowedUsers, status);

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
            var currentUser = User.Identity?.Name;

            var result = await EmailServices.addEmail(emailDTOs, currentUser);

            return ResponseStatusCode<Email>.GetApiResponseCode(result, "HttpPost");
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