using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class CategoryServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Category> Errors = new Errors<Category>();
    public CategoryServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Category>> getCategoryList()
    {
        MainResponse<Category> response = new MainResponse<Category>();
        var category = await _unitOfWork.Categories.GetAll();
        response.acceptedObjects = category.ToList();
        return response;
    }
    public async Task<MainResponse<Category>> getCategory(int id)
    {
        MainResponse<Category> response = new MainResponse<Category>();
        var category = await _unitOfWork.Categories.GetById(id);

        if (category == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Category> { category };
        return response;
    }
    public async Task<MainResponse<Category>> updateCategory(int id, List<CategoryDTO> categoryUpdated)
    {
        var response = new MainResponse<Category>();

        try
        {
            var validList = await ValidateDTO.CategoryDTO(categoryUpdated , true);

            var existingCategory = await _unitOfWork.Categories.GetFirst(a => a.Id == id);
            if (existingCategory is null)
            {
                response.errors.Add($"Cannot find Category with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Category>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Category. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Category>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingCategory);

            await _unitOfWork.Categories.Update(existingCategory);

            response.acceptedObjects.Add(existingCategory);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Category>>(validList.rejectedObjects));
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

    public async Task<MainResponse<Category>> addCategory(List<CategoryDTO> category)
    {
        MainResponse<Category> response = new MainResponse<Category>();
        try
        {
            var validList = await ValidateDTO.CategoryDTO(category);

            List<Category> categorylist = _mapper.Map<List<Category>>(validList.acceptedObjects);
            List<Category> rejectedcategory = _mapper.Map<List<Category>>(validList.rejectedObjects);
            if (categorylist != null && categorylist.Count() > 0)
            {
                var categorylists = await _unitOfWork.Categories.Add(categorylist);
                response.acceptedObjects = categorylist;
            }
            if (rejectedcategory != null && rejectedcategory.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedcategory;
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
    public async Task<MainResponse<Category>> deleteCategory(int id)
    {
        MainResponse<Category> response = new MainResponse<Category>();

        var category = await _unitOfWork.Categories.DeletePhysical(p => p.Id == id);

        if (category == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Category> { category.First() };
        return response;
    }
}
