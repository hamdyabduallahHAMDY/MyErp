using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class CashAndBanksServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<CashAndBanks> Errors = new Errors<CashAndBanks>();
    public CashAndBanksServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<CashAndBanks>> getCashAndBanksList()
    {
        MainResponse<CashAndBanks> response = new MainResponse<CashAndBanks>();
        var cashAndBanks = await _unitOfWork.CashAndBankss.GetAll();
        response.acceptedObjects = cashAndBanks.ToList();
        return response;
    }
    public async Task<MainResponse<CashAndBanks>> getCashAndBanks(int id)
    {
        MainResponse<CashAndBanks> response = new MainResponse<CashAndBanks>();
        var cashAndBanks = await _unitOfWork.CashAndBankss.GetById(id);

        if (cashAndBanks == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<CashAndBanks> { cashAndBanks };
        return response;
    }
    public async Task<MainResponse<CashAndBanks>> updateCashAndBanks(int id, List<CashAndBanksDTO> cashAndBanksUpdated)
    {
        var response = new MainResponse<CashAndBanks>();

        try
        {
            var validList = await ValidateDTO.CashAndBanksDTO(cashAndBanksUpdated , true);

            var existingCashAndBanks = await _unitOfWork.CashAndBankss.GetFirst(a => a.Id == id);
            if (existingCashAndBanks is null)
            {
                response.errors.Add($"Cannot find CashAndBanks with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<CashAndBanks>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update CashAndBanks. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<CashAndBanks>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingCashAndBanks);

            await _unitOfWork.CashAndBankss.Update(existingCashAndBanks);

            response.acceptedObjects.Add(existingCashAndBanks);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<CashAndBanks>>(validList.rejectedObjects));
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

    public async Task<MainResponse<CashAndBanks>> addCashAndBanks(List<CashAndBanksDTO> cashAndBanks)
    {
        MainResponse<CashAndBanks> response = new MainResponse<CashAndBanks>();
        try
        {
            var validList = await ValidateDTO.CashAndBanksDTO(cashAndBanks);

            List<CashAndBanks> cashAndBankslist = _mapper.Map<List<CashAndBanks>>(validList.acceptedObjects);
            List<CashAndBanks> rejectedcashAndBanks = _mapper.Map<List<CashAndBanks>>(validList.rejectedObjects);
            if (cashAndBankslist != null && cashAndBankslist.Count() > 0)
            {
                var cashAndBankslists = await _unitOfWork.CashAndBankss.Add(cashAndBankslist);
                response.acceptedObjects = cashAndBankslist;
            }
            if (rejectedcashAndBanks != null && rejectedcashAndBanks.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedcashAndBanks;
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
    public async Task<MainResponse<CashAndBanks>> deleteCashAndBanks(int id)
    {
        MainResponse<CashAndBanks> response = new MainResponse<CashAndBanks>();

        var cashAndBanks = await _unitOfWork.CashAndBankss.DeletePhysical(p => p.Id == id);

        if (cashAndBanks == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<CashAndBanks> { cashAndBanks.First() };
        return response;
    }
}
