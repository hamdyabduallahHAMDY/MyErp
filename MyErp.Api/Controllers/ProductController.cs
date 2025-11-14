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
    public class ProductController : Controller
    {
        ProductServices ProductServices;
        private readonly IMapper _mapper;

        public ProductController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            ProductServices = new ProductServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetProductlist()
        {
            var result = await ProductServices.getProductList();

            var resultWithStatusCode = ResponseStatusCode<Product>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await ProductServices.getProduct(id);
            var resultWithStatusCode = ResponseStatusCode<Product>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] List<ProductDTO> productupdated)
        {
            var result = await ProductServices.updateProduct(id, productupdated);
            var resultWithStatusCode = ResponseStatusCode<Product>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] List<ProductDTO> product)
        {
            var result = await ProductServices.addProduct(product);
            var resultWithStatusCode = ResponseStatusCode<Product>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await ProductServices.deleteProduct(id);
            var resultWithStatusCode = ResponseStatusCode<Product>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

