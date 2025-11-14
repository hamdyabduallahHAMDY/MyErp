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
    public class AreaController : Controller
    {
        AreaServices AreaServices;
        private readonly IMapper _mapper;

        public AreaController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            AreaServices = new AreaServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetArealist()
        {
            var result = await AreaServices.getAreaList();

            var resultWithStatusCode = ResponseStatusCode<Area>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetArea(int id)
        {
            var result = await AreaServices.getArea(id);
            var resultWithStatusCode = ResponseStatusCode<Area>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutArea(int id, [FromBody] List<AreaDTO> areaupdated)
        {
            var result = await AreaServices.updateArea(id, areaupdated);
            var resultWithStatusCode = ResponseStatusCode<Area>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddArea([FromBody] List<AreaDTO> area)
        {
            var result = await AreaServices.addArea(area);
            var resultWithStatusCode = ResponseStatusCode<Area>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var result = await AreaServices.deleteArea(id);
            var resultWithStatusCode = ResponseStatusCode<Area>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}
