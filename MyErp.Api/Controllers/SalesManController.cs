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
    public class SalesManController : Controller
    {
        SalesManServices SalesManServices;
        private readonly IMapper _mapper;

        public SalesManController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            SalesManServices = new SalesManServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetSalesManlist()
        {
            var result = await SalesManServices.getSalesManList();

            var resultWithStatusCode = ResponseStatusCode<SalesMan>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetSalesMan(int id)
        {
            var result = await SalesManServices.getSalesMan(id);
            var resultWithStatusCode = ResponseStatusCode<SalesMan>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutSalesMan(int id, [FromBody] List<SalesManDTO> salesManupdated)
        {
            var result = await SalesManServices.updateSalesMan(id, salesManupdated);
            var resultWithStatusCode = ResponseStatusCode<SalesMan>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddSalesMan([FromBody] List<SalesManDTO> salesMan)
        {
            var result = await SalesManServices.addSalesMan(salesMan);
            var resultWithStatusCode = ResponseStatusCode<SalesMan>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteSalesMan(int id)
        {
            var result = await SalesManServices.deleteSalesMan(id);
            var resultWithStatusCode = ResponseStatusCode<SalesMan>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

