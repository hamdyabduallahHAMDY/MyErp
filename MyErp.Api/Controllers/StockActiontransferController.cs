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
    public class StockActiontransferController : Controller
    {
        StockActiontransferServices StockActiontransferServices;
        private readonly IMapper _mapper;

        public StockActiontransferController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            StockActiontransferServices = new StockActiontransferServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetStockActiontransferlist()
        {
            var result = await StockActiontransferServices.getStockActiontransferList();

            var resultWithStatusCode = ResponseStatusCode<StockActiontransfer>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetStockActiontransfer(int id)
        {
            var result = await StockActiontransferServices.getStockActiontransfer(id);
            var resultWithStatusCode = ResponseStatusCode<StockActiontransfer>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutStockActiontransfer(int id, [FromBody] List<StockActiontransferDTO> stockActiontransferupdated)
        {
            var result = await StockActiontransferServices.updateStockActiontransfer(id, stockActiontransferupdated);
            var resultWithStatusCode = ResponseStatusCode<StockActiontransfer>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddStockActiontransfer([FromBody] List<StockActiontransferDTO> stockActiontransfer)
        {
            var result = await StockActiontransferServices.addStockActiontransfer(stockActiontransfer);
            var resultWithStatusCode = ResponseStatusCode<StockActiontransfer>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteStockActiontransfer(int id)
        {
            var result = await StockActiontransferServices.deleteStockActiontransfer(id);
            var resultWithStatusCode = ResponseStatusCode<StockActiontransfer>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

