using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class ContractServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    Errors<Contract> Errors = new Errors<Contract>();

    public ContractServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MainResponse<Contract>> getContractList()
    {
        MainResponse<Contract> response = new MainResponse<Contract>();

        var contracts = await _unitOfWork.Contracts.GetAll();

        response.acceptedObjects = contracts.ToList();

        return response;
    }

    public async Task<MainResponse<Contract>> getContract(int id)
    {
        MainResponse<Contract> response = new MainResponse<Contract>();

        var contract = await _unitOfWork.Contracts.GetById(id);

        if (contract == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Contract> { contract };

        return response;
    }

    public async Task<MainResponse<Contract>> addContract(List<ContractDTO> contracts)
    {
        MainResponse<Contract> response = new MainResponse<Contract>();

        try
        {
            var validList = await ValidateDTO.ContractDTO(contracts);

            List<Contract> contractList = _mapper.Map<List<Contract>>(validList.acceptedObjects);
            List<Contract> rejectedContracts = _mapper.Map<List<Contract>>(validList.rejectedObjects);

            if (contractList != null && contractList.Count > 0)
            {
                await _unitOfWork.Contracts.Add(contractList);
                response.acceptedObjects = contractList;
            }

            if (rejectedContracts != null && rejectedContracts.Count > 0)
            {
                response.rejectedObjects = rejectedContracts;
                response.errors = validList.errors;
            }

            return response;
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            response.errors.Add(ex.Message);
            if (ex.InnerException != null) response.errors.Add(ex.InnerException.Message);
            return response;
        }
    }

    public async Task<MainResponse<Contract>> updateContract(int id, List<ContractDTO> contractUpdated)
    {
        var response = new MainResponse<Contract>();

        try
        {
            var validList = await ValidateDTO.ContractDTO(contractUpdated, true);

            var existingContract = await _unitOfWork.Contracts.GetFirst(a => a.Id == id);

            if (existingContract is null)
            {
                response.errors.Add($"Cannot find Contract with ID {id}.");

                if (validList.errors?.Any() == true)
                    response.errors.AddRange(validList.errors);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Contract>>(validList.rejectedObjects));

                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Contract. Fix validation errors and try again.");

                if (validList.errors?.Any() == true)
                    response.errors.AddRange(validList.errors);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Contract>>(validList.rejectedObjects));

                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingContract);

            await _unitOfWork.Contracts.Update(existingContract);

            response.acceptedObjects.Add(existingContract);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Contract>>(validList.rejectedObjects));

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

    public async Task<MainResponse<Contract>> deleteContract(int id)
    {
        MainResponse<Contract> response = new MainResponse<Contract>();

        var contract = await _unitOfWork.Contracts.DeletePhysical(p => p.Id == id);

        if (contract == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Contract> { contract.First() };

        return response;
    }
    public async Task<MainResponse<Contract>> getContractsEndingInDays(int days)
    {
        MainResponse<Contract> response = new MainResponse<Contract>();

        try
        {
            if (days <= 0)
            {
                response.errors.Add("Days must be greater than 0.");
                return response;
            }

            DateTime today = DateTime.Now.Date;
            DateTime targetDate = today.AddDays(days);



            var x = await _unitOfWork.Contracts.GetAll();

            var contracts = await _unitOfWork.Contracts.GetAll(c =>
                c.EndDate >= today && c.EndDate <= targetDate
            );

            response.acceptedObjects = contracts.ToList();
            return response;
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            response.errors.Add(ex.Message);
            if (ex.InnerException != null)
                response.errors.Add(ex.InnerException.Message);

            return response;
        }
    }

}
