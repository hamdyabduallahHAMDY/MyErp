using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class StockActiontransferServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<StockActiontransfer> Errors = new Errors<StockActiontransfer>();
    public StockActiontransferServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<StockActiontransfer>> getStockActiontransferList()
    {
        MainResponse<StockActiontransfer> response = new MainResponse<StockActiontransfer>();
        var stockActiontransfer = await _unitOfWork.StockActiontransfers.GetAll();
        response.acceptedObjects = stockActiontransfer.ToList();
        return response;
    }
    public async Task<MainResponse<StockActiontransfer>> getStockActiontransfer(int id)
    {
        MainResponse<StockActiontransfer> response = new MainResponse<StockActiontransfer>();
        var stockActiontransfer = await _unitOfWork.StockActiontransfers.GetById(id);

        if (stockActiontransfer == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<StockActiontransfer> { stockActiontransfer };
        return response;
    }
    public async Task<MainResponse<StockActiontransfer>> updateStockActiontransfer(int id, List<StockActiontransferDTO> stockActiontransferUpdated)
    {
        var response = new MainResponse<StockActiontransfer>();

        try
        {
            var validList = await ValidateDTO.StockActiontransferDTO(stockActiontransferUpdated , true);

            var existingStockActiontransfer = await _unitOfWork.StockActiontransfers.GetFirst(a => a.Id == id);
            if (existingStockActiontransfer is null)
            {
                response.errors.Add($"Cannot find StockActiontransfer with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<StockActiontransfer>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update StockActiontransfer. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<StockActiontransfer>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingStockActiontransfer);

            await _unitOfWork.StockActiontransfers.Update(existingStockActiontransfer);

            response.acceptedObjects.Add(existingStockActiontransfer);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<StockActiontransfer>>(validList.rejectedObjects));
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

    public async Task<MainResponse<StockActiontransfer>> addStockActiontransfer(List<StockActiontransferDTO> stockActiontransfer)
    {
        MainResponse<StockActiontransfer> response = new MainResponse<StockActiontransfer>();
        try
        {
            var validList = await ValidateDTO.StockActiontransferDTO(stockActiontransfer);

            List<StockActiontransfer> stockActiontransferlist = _mapper.Map<List<StockActiontransfer>>(validList.acceptedObjects);
            List<StockActiontransfer> rejectedstockActiontransfer = _mapper.Map<List<StockActiontransfer>>(validList.rejectedObjects);
            if (stockActiontransferlist != null && stockActiontransferlist.Count() > 0)
            {
                var stockActiontransferlists = await _unitOfWork.StockActiontransfers.Add(stockActiontransferlist);
                response.acceptedObjects = stockActiontransferlist;
            }
            if (rejectedstockActiontransfer != null && rejectedstockActiontransfer.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedstockActiontransfer;
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
    public async Task<MainResponse<StockActiontransfer>> deleteStockActiontransfer(int id)
    {
        MainResponse<StockActiontransfer> response = new MainResponse<StockActiontransfer>();

        var stockActiontransfer = await _unitOfWork.StockActiontransfers.DeletePhysical(p => p.Id == id);

        if (stockActiontransfer == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<StockActiontransfer> { stockActiontransfer.First() };
        return response;
    }
}
