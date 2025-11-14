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
    public class OrdermedetailController : Controller
    {
        OrdermedetailServices OrdermedetailServices;
        private readonly IMapper _mapper;

        public OrdermedetailController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            OrdermedetailServices = new OrdermedetailServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetOrdermedetaillist()
        {
            var result = await OrdermedetailServices.getOrdermedetailList();

            var resultWithStatusCode = ResponseStatusCode<Ordermedetail>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetOrdermedetail(int id)
        {
            var result = await OrdermedetailServices.getOrdermedetail(id);
            var resultWithStatusCode = ResponseStatusCode<Ordermedetail>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutOrdermedetail(int id, [FromBody] List<OrdermedetailDTO> ordermedetailupdated)
        {
            var result = await OrdermedetailServices.updateOrdermedetail(id, ordermedetailupdated);
            var resultWithStatusCode = ResponseStatusCode<Ordermedetail>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddOrdermedetail([FromBody] List<OrdermedetailDTO> ordermedetail)
        {
            var result = await OrdermedetailServices.addOrdermedetail(ordermedetail);
            var resultWithStatusCode = ResponseStatusCode<Ordermedetail>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteOrdermedetail(int id)
        {
            var result = await OrdermedetailServices.deleteOrdermedetail(id);
            var resultWithStatusCode = ResponseStatusCode<Ordermedetail>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

