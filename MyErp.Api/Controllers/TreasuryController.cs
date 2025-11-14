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
    public class TreasuryController : Controller
    {
        TreasuryServices TreasuryServices;
        private readonly IMapper _mapper;

        public TreasuryController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            TreasuryServices = new TreasuryServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetTreasurylist()
        {
            var result = await TreasuryServices.getTreasuryList();

            var resultWithStatusCode = ResponseStatusCode<Treasury>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetTreasury(int id)
        {
            var result = await TreasuryServices.getTreasury(id);
            var resultWithStatusCode = ResponseStatusCode<Treasury>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutTreasury(int id, [FromBody] List<TreasuryDTO> treasuryupdated)
        {
            var result = await TreasuryServices.updateTreasury(id, treasuryupdated);
            var resultWithStatusCode = ResponseStatusCode<Treasury>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddTreasury([FromBody] List<TreasuryDTO> treasury)
        {
            var result = await TreasuryServices.addTreasury(treasury);
            var resultWithStatusCode = ResponseStatusCode<Treasury>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteTreasury(int id)
        {
            var result = await TreasuryServices.deleteTreasury(id);
            var resultWithStatusCode = ResponseStatusCode<Treasury>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

