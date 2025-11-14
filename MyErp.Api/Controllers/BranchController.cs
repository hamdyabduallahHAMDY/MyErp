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
    public class BranchController : Controller
    {
        BranchServices BranchServices;
        private readonly IMapper _mapper;

        public BranchController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            BranchServices = new BranchServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetBranchlist()
        {
            var result = await BranchServices.getBranchList();

            var resultWithStatusCode = ResponseStatusCode<Branch>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetBranch(int id)
        {
            var result = await BranchServices.getBranch(id);
            var resultWithStatusCode = ResponseStatusCode<Branch>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutBranch(int id, [FromBody] List<BranchDTO> branchupdated)
        {
            var result = await BranchServices.updateBranch(id, branchupdated);
            var resultWithStatusCode = ResponseStatusCode<Branch>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddBranch([FromBody] List<BranchDTO> branch)
        {
            var result = await BranchServices.addBranch(branch);
            var resultWithStatusCode = ResponseStatusCode<Branch>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await BranchServices.deleteBranch(id);
            var resultWithStatusCode = ResponseStatusCode<Branch>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

