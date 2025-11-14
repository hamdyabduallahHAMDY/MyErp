using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class AreaServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Area> Errors = new Errors<Area>();
    public AreaServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Area>> getAreaList()
    {
        MainResponse<Area> response = new MainResponse<Area>();
        var area = await _unitOfWork.Areas.GetAll();
        response.acceptedObjects = area.ToList();
        return response;
    }
    public async Task<MainResponse<Area>> getArea(int id)
    {
        MainResponse<Area> response = new MainResponse<Area>();
        var area = await _unitOfWork.Areas.GetById(id);

        if (area == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Area> { area };
        return response;
    }
    public async Task<MainResponse<Area>> updateArea(int id, List<AreaDTO> areaUpdated)
    {
        var response = new MainResponse<Area>();

        try
        {
            var validList = await ValidateDTO.AreaDTO(areaUpdated , true);

            // Load the existing entity once
            var existingArea = await _unitOfWork.Areas.GetFirst(a => a.Id == id);
            if (existingArea is null)
            {
                response.errors.Add($"Cannot find Area with ID {id}.");
                // Still return validation feedback if any
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Area>>(validList.rejectedObjects));
                return response;
            }

            // Expect exactly one accepted DTO for an update-by-id
            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Area. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Area>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            // Optional: enforce route id
            // if (dto.Id != 0 && dto.Id != id) response.errors.Add($"Payload id {dto.Id} doesn't match route id {id}.");
            // existingArea.Id = id; // keep the original id

            // ? Map DTO -> existing tracked entity
            _mapper.Map(dto, existingArea);

            // Persist
            await _unitOfWork.Areas.Update(existingArea);         // or Update(new List<Area>{ existingArea })

            response.acceptedObjects.Add(existingArea);

            // Attach validation feedback for any rejected DTOs
            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Area>>(validList.rejectedObjects));
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


    public async Task<MainResponse<Area>> addArea(List<AreaDTO> area)
    {
        MainResponse<Area> response = new MainResponse<Area>();
        try
        {
            var validList = await ValidateDTO.AreaDTO(area);

            List<Area> arealist = _mapper.Map<List<Area>>(validList.acceptedObjects);///Mapping<Product, ProductDTO>.get(validList.acceptedObjects);
            List<Area> rejectedarea = _mapper.Map<List<Area>>(validList.rejectedObjects);//Mapping<Product, ProductDTO>.update(validList.rejectedObjects,id);
            if (arealist != null && arealist.Count() > 0)
            {
                var arealists = await _unitOfWork.Areas.Add(arealist);
                response.acceptedObjects = arealist;
                //return response;
            }
            if (rejectedarea != null && rejectedarea.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedarea;
                response.errors = err;
                //return response;
            }
            return response;
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            return response;
        }

    }
    public async Task<MainResponse<Area>> deleteArea(int id)
    {
        MainResponse<Area> response = new MainResponse<Area>();

        var area = await _unitOfWork.Areas.DeletePhysical(p => p.Id == id);

        if (area == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Area> { area.First() };
        return response;
    }

    // 2. Get all related StockActionDetails

}


