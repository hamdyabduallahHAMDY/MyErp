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
    public class CashFlowController : Controller
    {
        CashFlowServices CashFlowServices;
        private readonly IMapper _mapper;

        public CashFlowController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            CashFlowServices = new CashFlowServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetCashFlowlist()
        {
            var result = await CashFlowServices.getCashFlowList();

            var resultWithStatusCode = ResponseStatusCode<CashFlow>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetCashFlow(int id)
        {
            var result = await CashFlowServices.getCashFlow(id);
            var resultWithStatusCode = ResponseStatusCode<CashFlow>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutCashFlow(int id, [FromBody] List<CashFlowDTO> cashFlowupdated)
        {
            var result = await CashFlowServices.updateCashFlow(id, cashFlowupdated);
            var resultWithStatusCode = ResponseStatusCode<CashFlow>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCashFlow([FromBody] List<CashFlowDTO> cashFlow)
        {
            var result = await CashFlowServices.addCashFlow(cashFlow);
            var resultWithStatusCode = ResponseStatusCode<CashFlow>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteCashFlow(int id)
        {
            var result = await CashFlowServices.deleteCashFlow(id);
            var resultWithStatusCode = ResponseStatusCode<CashFlow>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

