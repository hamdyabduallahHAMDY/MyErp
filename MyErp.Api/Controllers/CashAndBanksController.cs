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
    public class CashAndBanksController : Controller
    {
        CashAndBanksServices CashAndBanksServices;
        private readonly IMapper _mapper;

        public CashAndBanksController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            CashAndBanksServices = new CashAndBanksServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetCashAndBankslist()
        {
            var result = await CashAndBanksServices.getCashAndBanksList();

            var resultWithStatusCode = ResponseStatusCode<CashAndBanks>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetCashAndBanks(int id)
        {
            var result = await CashAndBanksServices.getCashAndBanks(id);
            var resultWithStatusCode = ResponseStatusCode<CashAndBanks>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutCashAndBanks(int id, [FromBody] List<CashAndBanksDTO> cashAndBanksupdated)
        {
            var result = await CashAndBanksServices.updateCashAndBanks(id, cashAndBanksupdated);
            var resultWithStatusCode = ResponseStatusCode<CashAndBanks>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCashAndBanks([FromBody] List<CashAndBanksDTO> cashAndBanks)
        {
            var result = await CashAndBanksServices.addCashAndBanks(cashAndBanks);
            var resultWithStatusCode = ResponseStatusCode<CashAndBanks>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteCashAndBanks(int id)
        {
            var result = await CashAndBanksServices.deleteCashAndBanks(id);
            var resultWithStatusCode = ResponseStatusCode<CashAndBanks>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

