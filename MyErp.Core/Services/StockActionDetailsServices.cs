using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class StockActionDetailsServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<StockActionDetails> Errors = new Errors<StockActionDetails>();
    public StockActionDetailsServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<StockActionDetails>> getStockActionDetailsList()
    {
        MainResponse<StockActionDetails> response = new MainResponse<StockActionDetails>();
        var stockActionDetails = await _unitOfWork.StockActionDetailss.GetAll();
        response.acceptedObjects = stockActionDetails.ToList();
        return response;
    }
    public async Task<MainResponse<StockActionDetails>> getStockActionDetails(int id)
    {
        MainResponse<StockActionDetails> response = new MainResponse<StockActionDetails>();
        var stockActionDetails = await _unitOfWork.StockActionDetailss.GetById(id);

        if (stockActionDetails == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<StockActionDetails> { stockActionDetails };
        return response;
    }
    public async Task<MainResponse<StockActionDetails>> updateStockActionDetails(int id, List<StockActionDetailsDTO> stockActionDetailsUpdated)
    {
        var response = new MainResponse<StockActionDetails>();

        try
        {
            var validList = await ValidateDTO.StockActionDetailsDTO(stockActionDetailsUpdated , true);

            var existingStockActionDetails = await _unitOfWork.StockActionDetailss.GetFirst(a => a.Id == id);
            if (existingStockActionDetails is null)
            {
                response.errors.Add($"Cannot find StockActionDetails with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<StockActionDetails>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update StockActionDetails. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<StockActionDetails>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingStockActionDetails);

            await _unitOfWork.StockActionDetailss.Update(existingStockActionDetails);

            response.acceptedObjects.Add(existingStockActionDetails);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<StockActionDetails>>(validList.rejectedObjects));
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

    public async Task<MainResponse<StockActionDetails>> addStockActionDetails(List<StockActionDetailsDTO> stockActionDetails)
    {
        MainResponse<StockActionDetails> response = new MainResponse<StockActionDetails>();
        try
        {
            var validList = await ValidateDTO.StockActionDetailsDTO(stockActionDetails);

            List<StockActionDetails> stockActionDetailslist = _mapper.Map<List<StockActionDetails>>(validList.acceptedObjects);
            List<StockActionDetails> rejectedstockActionDetails = _mapper.Map<List<StockActionDetails>>(validList.rejectedObjects);
            if (stockActionDetailslist != null && stockActionDetailslist.Count() > 0)
            {
                var stockActionDetailslists = await _unitOfWork.StockActionDetailss.Add(stockActionDetailslist);
                response.acceptedObjects = stockActionDetailslist;
            }
            if (rejectedstockActionDetails != null && rejectedstockActionDetails.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedstockActionDetails;
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
    public async Task<MainResponse<StockActionDetails>> deleteStockActionDetails(int id)
    {
        MainResponse<StockActionDetails> response = new MainResponse<StockActionDetails>();

        var stockActionDetails = await _unitOfWork.StockActionDetailss.DeletePhysical(p => p.Id == id);

        if (stockActionDetails == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<StockActionDetails> { stockActionDetails.First() };
        return response;
    }
}
