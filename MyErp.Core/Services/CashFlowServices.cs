using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class CashFlowServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<CashFlow> Errors = new Errors<CashFlow>();
    public CashFlowServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<CashFlow>> getCashFlowList()
    {
        MainResponse<CashFlow> response = new MainResponse<CashFlow>();
        var cashFlow = await _unitOfWork.CashFlows.GetAll();
        response.acceptedObjects = cashFlow.ToList();
        return response;
    }
    public async Task<MainResponse<CashFlow>> getCashFlow(int id)
    {
        MainResponse<CashFlow> response = new MainResponse<CashFlow>();
        var cashFlow = await _unitOfWork.CashFlows.GetById(id);

        if (cashFlow == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<CashFlow> { cashFlow };
        return response;
    }
    public async Task<MainResponse<CashFlow>> updateCashFlow(int id, List<CashFlowDTO> cashFlowUpdated)
    {
        var response = new MainResponse<CashFlow>();

        try
        {
            var validList = await ValidateDTO.CashFlowDTO(cashFlowUpdated , true);

            var existingCashFlow = await _unitOfWork.CashFlows.GetFirst(a => a.Id == id);
            if (existingCashFlow is null)
            {
                response.errors.Add($"Cannot find CashFlow with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<CashFlow>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update CashFlow. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<CashFlow>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingCashFlow);

            await _unitOfWork.CashFlows.Update(existingCashFlow);

            response.acceptedObjects.Add(existingCashFlow);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<CashFlow>>(validList.rejectedObjects));
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

    public async Task<MainResponse<CashFlow>> addCashFlow(List<CashFlowDTO> cashFlow)
    {
        MainResponse<CashFlow> response = new MainResponse<CashFlow>();
        try
        {
            var validList = await ValidateDTO.CashFlowDTO(cashFlow);

            List<CashFlow> cashFlowlist = _mapper.Map<List<CashFlow>>(validList.acceptedObjects);
            List<CashFlow> rejectedcashFlow = _mapper.Map<List<CashFlow>>(validList.rejectedObjects);
            if (cashFlowlist != null && cashFlowlist.Count() > 0)
            {
                var cashFlowlists = await _unitOfWork.CashFlows.Add(cashFlowlist);
                response.acceptedObjects = cashFlowlist;
            }
            if (rejectedcashFlow != null && rejectedcashFlow.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedcashFlow;
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
    public async Task<MainResponse<CashFlow>> deleteCashFlow(int id)
    {
        MainResponse<CashFlow> response = new MainResponse<CashFlow>();

        var cashFlow = await _unitOfWork.CashFlows.DeletePhysical(p => p.Id == id);

        if (cashFlow == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<CashFlow> { cashFlow.First() };
        return response;
    }
}
