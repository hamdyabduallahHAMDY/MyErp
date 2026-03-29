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
    public class LeadController : Controller
    {
        LeadServices LeadServices;
        private readonly IMapper _mapper;

        public LeadController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            LeadServices = new LeadServices(unitOfWork, _mapper);
        }

        // GET ALL LEADS
        [HttpGet("getAll")]
        public async Task<IActionResult> GetLeadList()
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

            // 3. Add current user to allowed list
            allowedUsers.Add(currentUser);




            var result = await LeadServices.GetAllLeads(allowedUsers);

            var resultWithStatusCode =
                ResponseStatusCode<Lead>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }


        [HttpGet("LeadsStatus")]
        public async Task<IActionResult> getLeadStatus(string UserId, DateTime dateFrom, DateTime dateTo)
        {
            var result = await LeadServices.GetLeadsStatus(UserId, dateFrom, dateTo);

            var resultWithStatusCode =
                ResponseStatusCode<LeadStatusPercentage>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        [HttpGet("LeadsCountry")]
        public async Task<IActionResult> getLeadsCountry(string UserId, DateTime dateFrom, DateTime dateTo)
        {
            var result = await LeadServices.GetLeadsCountries(UserId, dateFrom, dateTo);

            var resultWithStatusCode =
                ResponseStatusCode<LeadsCountry>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        [HttpGet("getLeadsStatusbyAssignedUser")]
        public async Task<IActionResult> getLeadsStatusbyAssignedUser()
        {
            var result = await LeadServices.GetLeadsStatusByAssignedUser();

            var resultWithStatusCode =
                ResponseStatusCode<LeadsStatusbyAssignedUser>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // GET LEAD BY ID
        [HttpGet("getById")]
        public async Task<IActionResult> GetLead(int id)
        {
            var result = await LeadServices.GetLead(id);

            var resultWithStatusCode =
                ResponseStatusCode<Lead>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // UPDATE LEAD
        [HttpPut("updateById")]
        public async Task<IActionResult> PutLead(int id, [FromBody] LeadDTO leadUpdated)
        {
            var result = await LeadServices.UpdateLead(id, leadUpdated);

            var resultWithStatusCode =
                ResponseStatusCode<Lead>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }
        [Authorize]
        // ADD LEAD
        [HttpPost("add")]
        public async Task<IActionResult> AddLead([FromBody] LeadDTO leadDTOs)
        {
            var currentUser = User.Identity.Name;
            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    message = "You must log in first."
                });
            }
            var result = await LeadServices.AddLead(leadDTOs , currentUser);

            var resultWithStatusCode =
                ResponseStatusCode<Lead>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }
        [Authorize]
        // IMPORT FROM EXCEL
        [HttpPost("addFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var result = await LeadServices.ImportFromExcel(file);

            var resultWithStatusCode =
                ResponseStatusCode<Lead>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        // DELETE LEAD
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteLead(int id)
        {
            var result = await LeadServices.DeleteLead(id);

            var resultWithStatusCode =
                ResponseStatusCode<Lead>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}