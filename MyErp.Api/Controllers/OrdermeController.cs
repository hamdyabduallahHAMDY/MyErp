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
            var result = await OrdermeServices.getOrdermeList();

            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetOrderme(int id)
        {
            var result = await OrdermeServices.getOrderme(id);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutOrderme(int id, [FromBody] List<OrdermeDTO> ordermeupdated)
        {
            var result = await OrdermeServices.updateOrderme(id, ordermeupdated);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddOrderme([FromBody] List<OrdermeDTO> orderme)
        {
            var result = await OrdermeServices.addOrderme(orderme);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteOrderme(int id)
        {
            var result = await OrdermeServices.deleteOrderme(id);
            var resultWithStatusCode = ResponseStatusCode<Orderme>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

