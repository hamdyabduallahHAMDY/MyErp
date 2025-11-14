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
    public class StockActionsController : Controller
    {
        StockActionsServices StockActionsServices;
        private readonly IMapper _mapper;

        public StockActionsController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            StockActionsServices = new StockActionsServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetStockActionslist()
        {
            var result = await StockActionsServices.getStockActionsList();

            var resultWithStatusCode = ResponseStatusCode<StockActions>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetStockActions(int id)
        {
            var result = await StockActionsServices.getStockActions(id);
            var resultWithStatusCode = ResponseStatusCode<StockActions>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutStockActions(int id, [FromBody] List<StockActionsDTO> stockActionsupdated)
        {
            var result = await StockActionsServices.updateStockActions(id, stockActionsupdated);
            var resultWithStatusCode = ResponseStatusCode<StockActions>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddStockActions([FromBody] List<StockActionsDTO> stockActions)
        {
            var result = await StockActionsServices.addStockActions(stockActions);
            var resultWithStatusCode = ResponseStatusCode<StockActions>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteStockActions(int id)
        {
            var result = await StockActionsServices.deleteStockActions(id);
            var resultWithStatusCode = ResponseStatusCode<StockActions>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

