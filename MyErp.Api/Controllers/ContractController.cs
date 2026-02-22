using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class ContractController : Controller
    {
        ContractServices ContractServices;
        private readonly IMapper _mapper;

        public ContractController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            ContractServices = new ContractServices(unitOfWork, _mapper);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetContractList()
        {
            var result = await ContractServices.getContractList();
            var resultWithStatusCode = ResponseStatusCode<Contract>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        [HttpGet("getById")]
        public async Task<IActionResult> GetContract(int id)
        {
            var result = await ContractServices.getContract(id);
            var resultWithStatusCode = ResponseStatusCode<Contract>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        [HttpPut("updateById")]
        public async Task<IActionResult> PutContract(int id, [FromBody] List<ContractDTO> contractUpdated)
        {
            var result = await ContractServices.updateContract(id, contractUpdated);
            var resultWithStatusCode = ResponseStatusCode<Contract>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddContract([FromBody] List<ContractDTO> contract)
        {
            var result = await ContractServices.addContract(contract);
            var resultWithStatusCode = ResponseStatusCode<Contract>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var result = await ContractServices.deleteContract(id);
            var resultWithStatusCode = ResponseStatusCode<Contract>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }

        [HttpGet("getEndingSoon")]
        public async Task<IActionResult> GetContractsEndingSoon(int days)
        {
            var result = await ContractServices.getContractsEndingInDays(days);
            var resultWithStatusCode = ResponseStatusCode<Contract>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

    }
}
