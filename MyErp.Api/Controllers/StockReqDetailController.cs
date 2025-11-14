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
    public class StockReqDetailController : Controller
    {
        StockReqDetailServices StockReqDetailServices;
        private readonly IMapper _mapper;

        public StockReqDetailController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            StockReqDetailServices = new StockReqDetailServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetStockReqDetaillist()
        {
            var result = await StockReqDetailServices.getStockReqDetailList();

            var resultWithStatusCode = ResponseStatusCode<StockReqDetail>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetStockReqDetail(int id)
        {
            var result = await StockReqDetailServices.getStockReqDetail(id);
            var resultWithStatusCode = ResponseStatusCode<StockReqDetail>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutStockReqDetail(int id, [FromBody] List<StockReqDetailDTO> stockReqDetailupdated)
        {
            var result = await StockReqDetailServices.updateStockReqDetail(id, stockReqDetailupdated);
            var resultWithStatusCode = ResponseStatusCode<StockReqDetail>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddStockReqDetail([FromBody] List<StockReqDetailDTO> stockReqDetail)
        {
            var result = await StockReqDetailServices.addStockReqDetail(stockReqDetail);
            var resultWithStatusCode = ResponseStatusCode<StockReqDetail>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteStockReqDetail(int id)
        {
            var result = await StockReqDetailServices.deleteStockReqDetail(id);
            var resultWithStatusCode = ResponseStatusCode<StockReqDetail>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

