using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class StockActionsServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<StockActions> Errors = new Errors<StockActions>();
    public StockActionsServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<StockActions>> getStockActionsList()
    {
        MainResponse<StockActions> response = new MainResponse<StockActions>();
        var stockActions = await _unitOfWork.StockActionss.GetAll();
        response.acceptedObjects = stockActions.ToList();
        return response;
    }
    public async Task<MainResponse<StockActions>> getStockActions(int id)
    {
        MainResponse<StockActions> response = new MainResponse<StockActions>();
        var stockActions = await _unitOfWork.StockActionss.GetById(id);

        if (stockActions == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<StockActions> { stockActions };
        return response;
    }
    public async Task<MainResponse<StockActions>> updateStockActions(int id, List<StockActionsDTO> stockActionsUpdated)
    {
        var response = new MainResponse<StockActions>();

        try
        {
            var validList = await ValidateDTO.StockActionsDTO(stockActionsUpdated , true);

            var existingStockActions = await _unitOfWork.StockActionss.GetFirst(a => a.Id == id);
            if (existingStockActions is null)
            {
                response.errors.Add($"Cannot find StockActions with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<StockActions>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update StockActions. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<StockActions>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingStockActions);

            await _unitOfWork.StockActionss.Update(existingStockActions);

            response.acceptedObjects.Add(existingStockActions);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<StockActions>>(validList.rejectedObjects));
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

    public async Task<MainResponse<StockActions>> addStockActions(List<StockActionsDTO> stockActions)
    {
        MainResponse<StockActions> response = new MainResponse<StockActions>();
        try
        {
            var validList = await ValidateDTO.StockActionsDTO(stockActions);

            List<StockActions> stockActionslist = _mapper.Map<List<StockActions>>(validList.acceptedObjects);
            List<StockActions> rejectedstockActions = _mapper.Map<List<StockActions>>(validList.rejectedObjects);
            if (stockActionslist != null && stockActionslist.Count() > 0)
            {
                var stockActionslists = await _unitOfWork.StockActionss.Add(stockActionslist);
                response.acceptedObjects = stockActionslist;
            }
            if (rejectedstockActions != null && rejectedstockActions.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedstockActions;
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
    public async Task<MainResponse<StockActions>> deleteStockActions(int id)
    {
        MainResponse<StockActions> response = new MainResponse<StockActions>();

        var stockActions = await _unitOfWork.StockActionss.DeletePhysical(p => p.Id == id);

        if (stockActions == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<StockActions> { stockActions.First() };
        return response;
    }
}
