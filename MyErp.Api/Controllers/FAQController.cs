using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class FAQController : Controller
    {
        FAQServices FAQServices;
        private readonly IMapper _mapper;

        public FAQController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            FAQServices = new FAQServices(unitOfWork, _mapper);
        }

        // ==============================
        // GET ALL FAQs
        // ==============================
        [HttpGet("getAll")]
        public async Task<IActionResult> GetFAQList()
        {
            var result = await FAQServices.getFAQsList();

            var resultWithStatusCode =
                ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // ==============================
        // GET FAQ BY ID
        // ==============================
        [HttpGet("getById")]
        public async Task<IActionResult> GetFAQ(int id)
        {
            var result = await FAQServices.getFAQ(id);

            var resultWithStatusCode =
                ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // ==============================
        // UPDATE FAQ
        // ==============================
        [HttpPut("updateById")]
        public async Task<IActionResult> PutFAQ(int id, [FromBody] List<FAQDTO> faqUpdated)
        {
            var result = await FAQServices.updateFAQ(id, faqUpdated);

            var resultWithStatusCode =
                ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }

        // ==============================
        // ADD FAQ
        // ==============================
        [HttpPost("add")]
        public async Task<IActionResult> AddFAQ([FromBody] List<FAQDTO> faqDTOs)
        {
            var result = await FAQServices.addFAQ(faqDTOs);

            var resultWithStatusCode =
                ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }

        // ==============================
        // DELETE FAQ
        // ==============================
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteFAQ(int id)
        {
            var result = await FAQServices.deleteFAQ(id);

            var resultWithStatusCode =
                ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}