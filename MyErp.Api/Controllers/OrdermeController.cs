using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services.ServicesUtilities;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class OrdermeController : Controller
    {
        OrdermeServices OrdermeServices;
        private readonly IMapper _mapper;

        public OrdermeController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            OrdermeServices = new OrdermeServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetOrdermelist()
        {
            var result = await OrdermeServices.GetOrdermeList();

            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetOrderme(int id)
        {
            var result = await OrdermeServices.GetOrderme(id);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutOrderme(int id, [FromBody] List<OrderCreateDTO> ordermeupdated)
        {
            var result = await OrdermeServices.UpdateStockActions(id, ordermeupdated);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddOrderme([FromBody] List<OrderCreateDTO> orderme)
        {
            var result = await OrdermeServices.AddOrdermes(orderme);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteOrderme(int id)
        {
            var result = await OrdermeServices.deleteStockActions(id);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

