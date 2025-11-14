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
    public class StockController : Controller
    {
        StockServices StockServices;
        private readonly IMapper _mapper;

        public StockController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            StockServices = new StockServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetStocklist()
        {
            var result = await StockServices.getStockList();

            var resultWithStatusCode = ResponseStatusCode<Stock>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetStock(int id)
        {
            var result = await StockServices.getStock(id);
            var resultWithStatusCode = ResponseStatusCode<Stock>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutStock(int id, [FromBody] List<StockDTO> stockupdated)
        {
            var result = await StockServices.updateStock(id, stockupdated);
            var resultWithStatusCode = ResponseStatusCode<Stock>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddStock([FromBody] List<StockDTO> stock)
        {
            var result = await StockServices.addStock(stock);
            var resultWithStatusCode = ResponseStatusCode<Stock>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var result = await StockServices.deleteStock(id);
            var resultWithStatusCode = ResponseStatusCode<Stock>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

