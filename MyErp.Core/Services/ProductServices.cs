using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class ProductServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Product> Errors = new Errors<Product>();
    public ProductServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Product>> getProductList()
    {
        MainResponse<Product> response = new MainResponse<Product>();
        var product = await _unitOfWork.Products.GetAll();
        response.acceptedObjects = product.ToList();
        return response;
    }
    public async Task<MainResponse<Product>> getProduct(int id)
    {
        MainResponse<Product> response = new MainResponse<Product>();
        var product = await _unitOfWork.Products.GetById(id);

        if (product == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Product> { product };
        return response;
    }
    public async Task<MainResponse<Product>> updateProduct(int id, List<ProductDTO> productUpdated)
    {
        var response = new MainResponse<Product>();

        try
        {
            var validList = await ValidateDTO.ProductDTO(productUpdated , true);

            var existingProduct = await _unitOfWork.Products.GetFirst(a => a.Id == id);
            if (existingProduct is null)
            {
                response.errors.Add($"Cannot find Product with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Product>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Product. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Product>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingProduct);

            await _unitOfWork.Products.Update(existingProduct);

            response.acceptedObjects.Add(existingProduct);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Product>>(validList.rejectedObjects));
            if (validList.errors?.Any() == true)
                response.errors.AddRange(validList.errors);
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            response.errors.Add(ex.Message);
            if (ex.InnerException != null) response.errors.Add(ex.InnerException.Message);
        }

        return response;
    }

    public async Task<MainResponse<Product>> addProduct(List<ProductDTO> product)
    {
        MainResponse<Product> response = new MainResponse<Product>();
        try
        {
            var validList = await ValidateDTO.ProductDTO(product);

            List<Product> productlist = _mapper.Map<List<Product>>(validList.acceptedObjects);
            List<Product> rejectedproduct = _mapper.Map<List<Product>>(validList.rejectedObjects);
            if (productlist != null && productlist.Count() > 0)
            {
                var productlists = await _unitOfWork.Products.Add(productlist);
                response.acceptedObjects = productlist;
            }
            if (rejectedproduct != null && rejectedproduct.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedproduct;
                response.errors = err;
            }
            return response;
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            return response;
        }
    }
    public async Task<MainResponse<Product>> deleteProduct(int id)
    {
        MainResponse<Product> response = new MainResponse<Product>();

        var product = await _unitOfWork.Products.DeletePhysical(p => p.Id == id);

        if (product == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Product> { product.First() };
        return response;
    }
}
