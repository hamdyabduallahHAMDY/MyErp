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
    public class CategoryController : Controller
    {
        CategoryServices CategoryServices;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            CategoryServices = new CategoryServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetCategorylist()
        {
            var result = await CategoryServices.getCategoryList();

            var resultWithStatusCode = ResponseStatusCode<Category>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var result = await CategoryServices.getCategory(id);
            var resultWithStatusCode = ResponseStatusCode<Category>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutCategory(int id, [FromBody] List<CategoryDTO> categoryupdated)
        {
            var result = await CategoryServices.updateCategory(id, categoryupdated);
            var resultWithStatusCode = ResponseStatusCode<Category>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddCategory([FromBody] List<CategoryDTO> category)
        {
            var result = await CategoryServices.addCategory(category);
            var resultWithStatusCode = ResponseStatusCode<Category>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await CategoryServices.deleteCategory(id);
            var resultWithStatusCode = ResponseStatusCode<Category>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

