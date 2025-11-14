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
    public class StockActionDetailsController : Controller
    {
        StockActionDetailsServices StockActionDetailsServices;
        private readonly IMapper _mapper;

        public StockActionDetailsController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            StockActionDetailsServices = new StockActionDetailsServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetStockActionDetailslist()
        {
            var result = await StockActionDetailsServices.getStockActionDetailsList();

            var resultWithStatusCode = ResponseStatusCode<StockActionDetails>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetStockActionDetails(int id)
        {
            var result = await StockActionDetailsServices.getStockActionDetails(id);
            var resultWithStatusCode = ResponseStatusCode<StockActionDetails>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutStockActionDetails(int id, [FromBody] List<StockActionDetailsDTO> stockActionDetailsupdated)
        {
            var result = await StockActionDetailsServices.updateStockActionDetails(id, stockActionDetailsupdated);
            var resultWithStatusCode = ResponseStatusCode<StockActionDetails>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddStockActionDetails([FromBody] List<StockActionDetailsDTO> stockActionDetails)
        {
            var result = await StockActionDetailsServices.addStockActionDetails(stockActionDetails);
            var resultWithStatusCode = ResponseStatusCode<StockActionDetails>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteStockActionDetails(int id)
        {
            var result = await StockActionDetailsServices.deleteStockActionDetails(id);
            var resultWithStatusCode = ResponseStatusCode<StockActionDetails>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

