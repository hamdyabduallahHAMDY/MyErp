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
    public class TicketController : Controller

    {
        TicketServices UserServices;
        private readonly IMapper _mapper;

        public TicketController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            UserServices = new TicketServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetUserlist()
        {
            var result = await UserServices.getTicketsList();

            var resultWithStatusCode = ResponseStatusCode<Ticket>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
            }
        [HttpGet("getById")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await UserServices.getTicket(id);
            var resultWithStatusCode = ResponseStatusCode<Ticket>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutUser(int id, [FromForm] List<TicketDTO> userupdated)
        {
            var result = await UserServices.updateTicket(id, userupdated);
            var resultWithStatusCode = ResponseStatusCode<Ticket>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        [RequestSizeLimit(1000000000)]
        public async Task<IActionResult> AddUser([FromForm] TicketDTO dto)
        {
            var result = await UserServices.addTicket(dto);
            var resultWithStatusCode = ResponseStatusCode<Ticket>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }


        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await UserServices.deleteUser(id);
            var resultWithStatusCode = ResponseStatusCode<Ticket>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}
