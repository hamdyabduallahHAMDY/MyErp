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
    public class StockReqController : Controller
    {
        StockReqServices StockReqServices;
        private readonly IMapper _mapper;

        public StockReqController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            StockReqServices = new StockReqServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetStockReqlist()
        {
            var result = await StockReqServices.getStockReqList();

            var resultWithStatusCode = ResponseStatusCode<StockReq>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetStockReq(int id)
        {
            var result = await StockReqServices.getStockReq(id);
            var resultWithStatusCode = ResponseStatusCode<StockReq>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutStockReq(int id, [FromBody] List<StockReqDTO> stockRequpdated)
        {
            var result = await StockReqServices.updateStockReq(id, stockRequpdated);
            var resultWithStatusCode = ResponseStatusCode<StockReq>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddStockReq([FromBody] List<StockReqDTO> stockReq)
        {
            var result = await StockReqServices.addStockReq(stockReq);
            var resultWithStatusCode = ResponseStatusCode<StockReq>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteStockReq(int id)
        {
            var result = await StockReqServices.deleteStockReq(id);
            var resultWithStatusCode = ResponseStatusCode<StockReq>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

