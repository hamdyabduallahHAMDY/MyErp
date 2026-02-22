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
    public class UserSessionController : Controller
    {
        UserSessionServices UserSessionServices;
        private readonly IMapper _mapper;

        public UserSessionController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            UserSessionServices = new UserSessionServices(unitOfWork, _mapper);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetSessionsList()
        {
            var result = await UserSessionServices.GetSession();
            var resultWithStatusCode = ResponseStatusCode<UserSession>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetSession(int id)
        {
            var result = await UserSessionServices.GetSessionID(id);
            var resultWithStatusCode = ResponseStatusCode<UserSession>.GetApiResponseCode(result, "HttpGet");
            return resultWithStatusCode;
        }

        [HttpPost("AddLog/in/out")]
        public async Task<IActionResult> Login([FromBody] List<UserSessionDTO> dto)
        {
            var result = await UserSessionServices.addSession(dto);
            var resultWithStatusCode = ResponseStatusCode<UserSession>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }


        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var result = await UserSessionServices.deleteSession(id);
            var resultWithStatusCode = ResponseStatusCode<UserSession>.GetApiResponseCode(result, "HttpDelete");
            return resultWithStatusCode;
        }
    }
}
