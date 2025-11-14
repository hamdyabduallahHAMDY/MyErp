using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class StockReqDetailServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<StockReqDetail> Errors = new Errors<StockReqDetail>();
    public StockReqDetailServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<StockReqDetail>> getStockReqDetailList()
    {
        MainResponse<StockReqDetail> response = new MainResponse<StockReqDetail>();
        var stockReqDetail = await _unitOfWork.StockReqDetails.GetAll();
        response.acceptedObjects = stockReqDetail.ToList();
        return response;
    }
    public async Task<MainResponse<StockReqDetail>> getStockReqDetail(int id)
    {
        MainResponse<StockReqDetail> response = new MainResponse<StockReqDetail>();
        var stockReqDetail = await _unitOfWork.StockReqDetails.GetById(id);

        if (stockReqDetail == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<StockReqDetail> { stockReqDetail };
        return response;
    }
    public async Task<MainResponse<StockReqDetail>> updateStockReqDetail(int id, List<StockReqDetailDTO> stockReqDetailUpdated)
    {
        var response = new MainResponse<StockReqDetail>();

        try
        {
            var validList = await ValidateDTO.StockReqDetailDTO(stockReqDetailUpdated , true);

            var existingStockReqDetail = await _unitOfWork.StockReqDetails.GetFirst(a => a.Id == id);
            if (existingStockReqDetail is null)
            {
                response.errors.Add($"Cannot find StockReqDetail with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<StockReqDetail>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update StockReqDetail. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<StockReqDetail>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingStockReqDetail);

            await _unitOfWork.StockReqDetails.Update(existingStockReqDetail);

            response.acceptedObjects.Add(existingStockReqDetail);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<StockReqDetail>>(validList.rejectedObjects));
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

    public async Task<MainResponse<StockReqDetail>> addStockReqDetail(List<StockReqDetailDTO> stockReqDetail)
    {
        MainResponse<StockReqDetail> response = new MainResponse<StockReqDetail>();
        try
        {
            var validList = await ValidateDTO.StockReqDetailDTO(stockReqDetail);

            List<StockReqDetail> stockReqDetaillist = _mapper.Map<List<StockReqDetail>>(validList.acceptedObjects);
            List<StockReqDetail> rejectedstockReqDetail = _mapper.Map<List<StockReqDetail>>(validList.rejectedObjects);
            if (stockReqDetaillist != null && stockReqDetaillist.Count() > 0)
            {
                var stockReqDetaillists = await _unitOfWork.StockReqDetails.Add(stockReqDetaillist);
                response.acceptedObjects = stockReqDetaillist;
            }
            if (rejectedstockReqDetail != null && rejectedstockReqDetail.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedstockReqDetail;
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
    public async Task<MainResponse<StockReqDetail>> deleteStockReqDetail(int id)
    {
        MainResponse<StockReqDetail> response = new MainResponse<StockReqDetail>();

        var stockReqDetail = await _unitOfWork.StockReqDetails.DeletePhysical(p => p.Id == id);

        if (stockReqDetail == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<StockReqDetail> { stockReqDetail.First() };
        return response;
    }
}
