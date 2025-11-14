using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class StockTakingServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<StockTaking> Errors = new Errors<StockTaking>();
    public StockTakingServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<StockTaking>> getStockTakingList()
    {
        MainResponse<StockTaking> response = new MainResponse<StockTaking>();
        var stockTaking = await _unitOfWork.StockTakings.GetAll();
        response.acceptedObjects = stockTaking.ToList();
        return response;
    }
    public async Task<MainResponse<StockTaking>> getStockTaking(int id)
    {
        MainResponse<StockTaking> response = new MainResponse<StockTaking>();
        var stockTaking = await _unitOfWork.StockTakings.GetById(id);

        if (stockTaking == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<StockTaking> { stockTaking };
        return response;
    }
    public async Task<MainResponse<StockTaking>> updateStockTaking(int id, List<StockTakingDTO> stockTakingUpdated)
    {
        var response = new MainResponse<StockTaking>();

        try
        {
            var validList = await ValidateDTO.StockTakingDTO(stockTakingUpdated , true);

            var existingStockTaking = await _unitOfWork.StockTakings.GetFirst(a => a.Id == id);
            if (existingStockTaking is null)
            {
                response.errors.Add($"Cannot find StockTaking with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<StockTaking>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update StockTaking. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<StockTaking>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingStockTaking);

            await _unitOfWork.StockTakings.Update(existingStockTaking);

            response.acceptedObjects.Add(existingStockTaking);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<StockTaking>>(validList.rejectedObjects));
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

    public async Task<MainResponse<StockTaking>> addStockTaking(List<StockTakingDTO> stockTaking)
    {
        MainResponse<StockTaking> response = new MainResponse<StockTaking>();
        try
        {
            var validList = await ValidateDTO.StockTakingDTO(stockTaking);

            List<StockTaking> stockTakinglist = _mapper.Map<List<StockTaking>>(validList.acceptedObjects);
            List<StockTaking> rejectedstockTaking = _mapper.Map<List<StockTaking>>(validList.rejectedObjects);
            if (stockTakinglist != null && stockTakinglist.Count() > 0)
            {
                var stockTakinglists = await _unitOfWork.StockTakings.Add(stockTakinglist);
                response.acceptedObjects = stockTakinglist;
            }
            if (rejectedstockTaking != null && rejectedstockTaking.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedstockTaking;
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
    public async Task<MainResponse<StockTaking>> deleteStockTaking(int id)
    {
        MainResponse<StockTaking> response = new MainResponse<StockTaking>();

        var stockTaking = await _unitOfWork.StockTakings.DeletePhysical(p => p.Id == id);

        if (stockTaking == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<StockTaking> { stockTaking.First() };
        return response;
    }
}
