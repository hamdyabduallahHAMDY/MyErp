using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class StockServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Stock> Errors = new Errors<Stock>();
    public StockServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Stock>> getStockList()
    {
        MainResponse<Stock> response = new MainResponse<Stock>();
        var stock = await _unitOfWork.Stocks.GetAll();
        response.acceptedObjects = stock.ToList();
        return response;
    }
    public async Task<MainResponse<Stock>> getStock(int id)
    {
        MainResponse<Stock> response = new MainResponse<Stock>();
        var stock = await _unitOfWork.Stocks.GetById(id);

        if (stock == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Stock> { stock };
        return response;
    }
    public async Task<MainResponse<Stock>> updateStock(int id, List<StockDTO> stockUpdated)
    {
        var response = new MainResponse<Stock>();

        try
        {
            var validList = await ValidateDTO.StockDTO(stockUpdated , true);

            var existingStock = await _unitOfWork.Stocks.GetFirst(a => a.Id == id);
            if (existingStock is null)
            {
                response.errors.Add($"Cannot find Stock with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Stock>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Stock. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Stock>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingStock);

            await _unitOfWork.Stocks.Update(existingStock);

            response.acceptedObjects.Add(existingStock);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Stock>>(validList.rejectedObjects));
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

    public async Task<MainResponse<Stock>> addStock(List<StockDTO> stock)
    {
        MainResponse<Stock> response = new MainResponse<Stock>();
        try
        {
            var validList = await ValidateDTO.StockDTO(stock);

            List<Stock> stocklist = _mapper.Map<List<Stock>>(validList.acceptedObjects);
            List<Stock> rejectedstock = _mapper.Map<List<Stock>>(validList.rejectedObjects);
            if (stocklist != null && stocklist.Count() > 0)
            {
                var stocklists = await _unitOfWork.Stocks.Add(stocklist);
                response.acceptedObjects = stocklist;
            }
            if (rejectedstock != null && rejectedstock.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedstock;
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
    public async Task<MainResponse<Stock>> deleteStock(int id)
    {
        MainResponse<Stock> response = new MainResponse<Stock>();

        var stock = await _unitOfWork.Stocks.DeletePhysical(p => p.Id == id);

        if (stock == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Stock> { stock.First() };
        return response;
    }
}
