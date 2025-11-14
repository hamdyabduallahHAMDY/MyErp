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
    public class CurrencyController : Controller
    {
        CurrencyServices CurrencyServices;
        private readonly IMapper _mapper;

        public CurrencyController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            CurrencyServices = new CurrencyServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetCurrencylist()
        {
            var result = await CurrencyServices.getCurrencyList();

            var resultWithStatusCode = ResponseStatusCode<Currency>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetCurrency(int id)
        {
            var result = await CurrencyServices.getCurrency(id);
            var resultWithStatusCode = ResponseStatusCode<Currency>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutCurrency(int id, [FromBody] List<CurrencyDTO> currencyupdated)
        {
            var result = await CurrencyServices.updateCurrency(id, currencyupdated);
            var resultWithStatusCode = ResponseStatusCode<Currency>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCurrency([FromBody] List<CurrencyDTO> currency)
        {
            var result = await CurrencyServices.addCurrency(currency);
            var resultWithStatusCode = ResponseStatusCode<Currency>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteCurrency(int id)
        {
            var result = await CurrencyServices.deleteCurrency(id);
            var resultWithStatusCode = ResponseStatusCode<Currency>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

