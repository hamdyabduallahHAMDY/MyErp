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
    public class DocumentController : Controller
    {
        DocumentServices DocumentServices;
        private readonly IMapper _mapper;

        public DocumentController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            DocumentServices = new DocumentServices(unitOfWork, _mapper);
        }

        // GET ALL
        [HttpGet("getAll")]
        public async Task<IActionResult> GetDocumentList()
        {
            var result = await DocumentServices.getDocumentList();
            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpGet");

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
            var result = await DocumentServices.updateDocument(id, documentUpdated);

            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpPut");

            return resultWithStatusCode;
        }

        // ADD
        [HttpPost("add")]
        public async Task<IActionResult> AddDocument([FromForm] DocumentDTO documents)
        {
            // Get API root directory
            var apiRootPath = Directory.GetCurrentDirectory();

            var result = await DocumentServices.addDocument(documents, apiRootPath);

            var resultWithStatusCode =
                ResponseStatusCode<Document>.GetApiResponseCode(result, "HttpPost");

            return resultWithStatusCode;
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
    }
}