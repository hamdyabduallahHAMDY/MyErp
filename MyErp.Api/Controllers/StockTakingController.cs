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
    public class StockTakingController : Controller
    {
        StockTakingServices StockTakingServices;
        private readonly IMapper _mapper;

        public StockTakingController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            StockTakingServices = new StockTakingServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetStockTakinglist()
        {
            var result = await StockTakingServices.getStockTakingList();

            var resultWithStatusCode = ResponseStatusCode<StockTaking>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetStockTaking(int id)
        {
            var result = await StockTakingServices.getStockTaking(id);
            var resultWithStatusCode = ResponseStatusCode<StockTaking>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutStockTaking(int id, [FromBody] List<StockTakingDTO> stockTakingupdated)
        {
            var result = await StockTakingServices.updateStockTaking(id, stockTakingupdated);
            var resultWithStatusCode = ResponseStatusCode<StockTaking>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddStockTaking([FromBody] List<StockTakingDTO> stockTaking)
        {
            var result = await StockTakingServices.addStockTaking(stockTaking);
            var resultWithStatusCode = ResponseStatusCode<StockTaking>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteStockTaking(int id)
        {
            var result = await StockTakingServices.deleteStockTaking(id);
            var resultWithStatusCode = ResponseStatusCode<StockTaking>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

