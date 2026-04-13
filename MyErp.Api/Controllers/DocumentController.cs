using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
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
    public class DocumentController : Controller
    {
        DocumentServices DocumentServices;
        private readonly IMapper _mapper;
        private readonly RightsModelServices _accessService;

        public DocumentController(ApplicationDbContext dBContext, IMapper mapper , RightsModelServices accessService)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            DocumentServices = new DocumentServices(unitOfWork, _mapper);
            _accessService = accessService;
        }

        // GET ALL
        [Authorize]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetDocumentList()
        {
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);
            var result = await DocumentServices.getDocumentList(usertype);
            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            var result = await DocumentServices.deleteAll();
            var resultWithStatusCode = ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
        // GET BY ID
        [HttpGet("getById")]
        public async Task<IActionResult> GetDocument(int id)
        {
            var result = await DocumentServices.getDocument(id);
            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }

        // UPDATE
        [HttpPut("updateById")]
        public async Task<IActionResult> PutDocument(
            int id,
            [FromForm] DocumentDTO documentUpdated)
        {
            var apiRootPath = Directory.GetCurrentDirectory();

            var result = await DocumentServices.updateTicket(id, documentUpdated, apiRootPath);

            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }

        // ADD
        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddDocument([FromForm] DocumentDTO documents)
        {
            // Get API root directory
            var (currentUser, allowedUsers, isAuth, usertype) = _accessService.GetAccessData(User);

            var apiRootPath = Directory.GetCurrentDirectory();

            var result = await DocumentServices.addDocument(documents, apiRootPath,usertype , currentUser);

            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }
        [Authorize]
        [HttpPost("importExcel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var createdby = User.Identity.Name;
            var result = await DocumentServices.ImportFromExcel(file, createdby);

            var resultWithStatusCode =
                ResponseStatusCode<Document>
                .GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
        }
        [HttpGet("template/document")]
        public async Task<IActionResult> DownloadDocumentTemplate()
        {
            var fileBytes = await DocumentServices.GenerateDocumentExcelTemplate();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Document_Template.xlsx"
            );
        }
        // DELETE
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var result = await DocumentServices.deleteDocument(id);

            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload_Document", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }
        [HttpDelete("deleteGroupById")]
        public async Task<IActionResult> DeleteGroupCust(List<int> id)
        {
            var result = await DocumentServices.deleteGroup(id);

            var resultWithStatusCode = ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}