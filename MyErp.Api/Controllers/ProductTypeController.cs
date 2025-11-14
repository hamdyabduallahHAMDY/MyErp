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
    public class ProductTypeController : Controller
    {
        ProductTypeServices ProductTypeServices;
        private readonly IMapper _mapper;

        public ProductTypeController(ApplicationDbContext dBContext, IMapper mapper)
        {
            UnitOfWork unitOfWork = new UnitOfWork(dBContext);
            _mapper = mapper;
            ProductTypeServices = new ProductTypeServices(unitOfWork, _mapper);
        }
        [HttpGet("getAll")]
        public async Task<IActionResult> GetProductTypelist()
        {
            var result = await ProductTypeServices.getProductTypeList();

            var resultWithStatusCode = ResponseStatusCode<ProductType>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpGet("getById")]
        public async Task<IActionResult> GetProductType(int id)
        {
            var result = await ProductTypeServices.getProductType(id);
            var resultWithStatusCode = ResponseStatusCode<ProductType>.GetApiResponseCode(result, "HttpGet");

            return resultWithStatusCode;
        }
        [HttpPut("updateById")]
        public async Task<IActionResult> PutProductType(int id, [FromBody] List<ProductTypeDTO> productTypeupdated)
        {
            var result = await ProductTypeServices.updateProductType(id, productTypeupdated);
            var resultWithStatusCode = ResponseStatusCode<ProductType>.GetApiResponseCode(result, "HttpPut");
            return resultWithStatusCode;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddProductType([FromBody] List<ProductTypeDTO> productType)
        {
            var result = await ProductTypeServices.addProductType(productType);
            var resultWithStatusCode = ResponseStatusCode<ProductType>.GetApiResponseCode(result, "HttpPost");
            return resultWithStatusCode;
        }
        [HttpDelete("deleteById")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var result = await ProductTypeServices.deleteProductType(id);
            var resultWithStatusCode = ResponseStatusCode<ProductType>.GetApiResponseCode(result, "HttpDelete");

            return resultWithStatusCode;
        }
    }
}

