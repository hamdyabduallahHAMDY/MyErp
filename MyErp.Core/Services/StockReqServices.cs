using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class StockReqServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<StockReq> Errors = new Errors<StockReq>();
    public StockReqServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<StockReq>> getStockReqList()
    {
        MainResponse<StockReq> response = new MainResponse<StockReq>();
        var stockReq = await _unitOfWork.StockReqs.GetAll();
        response.acceptedObjects = stockReq.ToList();
        return response;
    }
    public async Task<MainResponse<StockReq>> getStockReq(int id)
    {
        MainResponse<StockReq> response = new MainResponse<StockReq>();
        var stockReq = await _unitOfWork.StockReqs.GetById(id);

        if (stockReq == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<StockReq> { stockReq };
        return response;
    }
    public async Task<MainResponse<StockReq>> updateStockReq(int id, List<StockReqDTO> stockReqUpdated)
    {
        var response = new MainResponse<StockReq>();

        try
        {
            var validList = await ValidateDTO.StockReqDTO(stockReqUpdated , true);

            var existingStockReq = await _unitOfWork.StockReqs.GetFirst(a => a.Id == id);
            if (existingStockReq is null)
            {
                response.errors.Add($"Cannot find StockReq with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<StockReq>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update StockReq. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<StockReq>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingStockReq);

            await _unitOfWork.StockReqs.Update(existingStockReq);

            response.acceptedObjects.Add(existingStockReq);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<StockReq>>(validList.rejectedObjects));
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

    public async Task<MainResponse<StockReq>> addStockReq(List<StockReqDTO> stockReq)
    {
        MainResponse<StockReq> response = new MainResponse<StockReq>();
        try
        {
            var validList = await ValidateDTO.StockReqDTO(stockReq);

            List<StockReq> stockReqlist = _mapper.Map<List<StockReq>>(validList.acceptedObjects);
            List<StockReq> rejectedstockReq = _mapper.Map<List<StockReq>>(validList.rejectedObjects);
            if (stockReqlist != null && stockReqlist.Count() > 0)
            {
                var stockReqlists = await _unitOfWork.StockReqs.Add(stockReqlist);
                response.acceptedObjects = stockReqlist;
            }
            if (rejectedstockReq != null && rejectedstockReq.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedstockReq;
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
    public async Task<MainResponse<StockReq>> deleteStockReq(int id)
    {
        MainResponse<StockReq> response = new MainResponse<StockReq>();

        var stockReq = await _unitOfWork.StockReqs.DeletePhysical(p => p.Id == id);

        if (stockReq == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<StockReq> { stockReq.First() };
        return response;
    }
}
