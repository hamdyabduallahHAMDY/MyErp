using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class CurrencyServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Currency> Errors = new Errors<Currency>();
    public CurrencyServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Currency>> getCurrencyList()
    {
        MainResponse<Currency> response = new MainResponse<Currency>();
        var currency = await _unitOfWork.Currencys.GetAll();
        response.acceptedObjects = currency.ToList();
        return response;
    }
    public async Task<MainResponse<Currency>> getCurrency(int id)
    {
        MainResponse<Currency> response = new MainResponse<Currency>();
        var currency = await _unitOfWork.Currencys.GetById(id);

        if (currency == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Currency> { currency };
        return response;
    }
    public async Task<MainResponse<Currency>> updateCurrency(int id, List<CurrencyDTO> currencyUpdated)
    {
        var response = new MainResponse<Currency>();

        try
        {
            var validList = await ValidateDTO.CurrencyDTO(currencyUpdated , true);

            var existingCurrency = await _unitOfWork.Currencys.GetFirst(a => a.Id == id);
            if (existingCurrency is null)
            {
                response.errors.Add($"Cannot find Currency with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Currency>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Currency. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Currency>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingCurrency);

            await _unitOfWork.Currencys.Update(existingCurrency);

            response.acceptedObjects.Add(existingCurrency);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Currency>>(validList.rejectedObjects));
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

    public async Task<MainResponse<Currency>> addCurrency(List<CurrencyDTO> currency)
    {
        MainResponse<Currency> response = new MainResponse<Currency>();
        try
        {
            var validList = await ValidateDTO.CurrencyDTO(currency);

            List<Currency> currencylist = _mapper.Map<List<Currency>>(validList.acceptedObjects);
            List<Currency> rejectedcurrency = _mapper.Map<List<Currency>>(validList.rejectedObjects);
            if (currencylist != null && currencylist.Count() > 0)
            {
                var currencylists = await _unitOfWork.Currencys.Add(currencylist);
                response.acceptedObjects = currencylist;
            }
            if (rejectedcurrency != null && rejectedcurrency.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedcurrency;
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
    public async Task<MainResponse<Currency>> deleteCurrency(int id)
    {
        MainResponse<Currency> response = new MainResponse<Currency>();

        var currency = await _unitOfWork.Currencys.DeletePhysical(p => p.Id == id);

        if (currency == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Currency> { currency.First() };
        return response;
    }
}
