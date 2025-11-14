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
    public class UserController : Controller
    {
        UserServices UserServices;
        private readonly IMapper _mapper;

        public UserController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            UserServices = new UserServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetUserlist()
        {
            var result = await UserServices.getUserList();

            var resultWithStatusCode = ResponseStatusCode<User>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await UserServices.getUser(id);
            var resultWithStatusCode = ResponseStatusCode<User>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutUser(int id, [FromBody] List<UserDTO> userupdated)
        {
            var result = await UserServices.updateUser(id, userupdated);
            var resultWithStatusCode = ResponseStatusCode<User>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] List<UserDTO> user)
        {
            var result = await UserServices.addUser(user);
            var resultWithStatusCode = ResponseStatusCode<User>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await UserServices.deleteUser(id);
            var resultWithStatusCode = ResponseStatusCode<User>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

