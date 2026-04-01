using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyErp.Core.DTO;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using MyErp.Core.Services;
using MyErp.EF.DataAccess;
using MyErp.EF.Repositories;
using OfficeOpenXml;

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
        public async Task<IActionResult> PutFAQ(int id, [FromForm] FAQDTO faqUpdated)
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
        public async Task<IActionResult> AddFAQ([FromForm] FAQDTO faqDTOs)
        {
            var result = await FAQServices.addFAQ(faqDTOs);

            var resultWithStatusCode =
                ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }
        [Authorize]
        [HttpPost("addFromExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var created = User.Identity.Name;
            var result = await FAQServices.ImportFromExcel(file, created);
            var resultWithStatusCode = ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpPost");
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
        [HttpGet("template/faq")]
        public async Task<IActionResult> DownloadFAQTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("FAQ Template");

            // ================= HEADERS =================
            worksheet.Cells[1, 1].Value = "Error";
            worksheet.Cells[1, 2].Value = "Details";

            // ================= DEMO ROW =================
            worksheet.Cells[2, 1].Value = "Login Failed";
            worksheet.Cells[2, 2].Value = "Incorrect username or password";

            // ================= STYLING =================
            using (var header = worksheet.Cells[1, 1, 1, 2])
            {
                header.Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();

            var fileBytes = await package.GetAsByteArrayAsync();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "FAQ_Template.xlsx"
            );
        }


        [HttpDelete("deleteGroupById")]
        public async Task<IActionResult> DeleteGroupCust(List<int> id)
        {
            var result = await FAQServices.deleteGroup(id);

            var resultWithStatusCode = ResponseStatusCode<FAQ>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }



    }



}