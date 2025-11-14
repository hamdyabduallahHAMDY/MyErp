using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class ProductTypeServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<ProductType> Errors = new Errors<ProductType>();
    public ProductTypeServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<ProductType>> getProductTypeList()
    {
        MainResponse<ProductType> response = new MainResponse<ProductType>();
        var productType = await _unitOfWork.ProductTypes.GetAll();
        response.acceptedObjects = productType.ToList();
        return response;
    }
    public async Task<MainResponse<ProductType>> getProductType(int id)
    {
        MainResponse<ProductType> response = new MainResponse<ProductType>();
        var productType = await _unitOfWork.ProductTypes.GetById(id);

        if (productType == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<ProductType> { productType };
        return response;
    }
    public async Task<MainResponse<ProductType>> updateProductType(int id, List<ProductTypeDTO> productTypeUpdated)
    {
        var response = new MainResponse<ProductType>();

        try
        {
            var validList = await ValidateDTO.ProductTypeDTO(productTypeUpdated , true);

            var existingProductType = await _unitOfWork.ProductTypes.GetFirst(a => a.Id == id);
            if (existingProductType is null)
            {
                response.errors.Add($"Cannot find ProductType with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<ProductType>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update ProductType. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<ProductType>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingProductType);

            await _unitOfWork.ProductTypes.Update(existingProductType);

            response.acceptedObjects.Add(existingProductType);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<ProductType>>(validList.rejectedObjects));
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

    public async Task<MainResponse<ProductType>> addProductType(List<ProductTypeDTO> productType)
    {
        MainResponse<ProductType> response = new MainResponse<ProductType>();
        try
        {
            var validList = await ValidateDTO.ProductTypeDTO(productType);

            List<ProductType> productTypelist = _mapper.Map<List<ProductType>>(validList.acceptedObjects);
            List<ProductType> rejectedproductType = _mapper.Map<List<ProductType>>(validList.rejectedObjects);
            if (productTypelist != null && productTypelist.Count() > 0)
            {
                var productTypelists = await _unitOfWork.ProductTypes.Add(productTypelist);
                response.acceptedObjects = productTypelist;
            }
            if (rejectedproductType != null && rejectedproductType.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedproductType;
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
    public async Task<MainResponse<ProductType>> deleteProductType(int id)
    {
        MainResponse<ProductType> response = new MainResponse<ProductType>();

        var productType = await _unitOfWork.ProductTypes.DeletePhysical(p => p.Id == id);

        if (productType == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<ProductType> { productType.First() };
        return response;
    }
}
