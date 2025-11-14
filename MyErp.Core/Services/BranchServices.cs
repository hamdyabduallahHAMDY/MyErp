using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class BranchServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Branch> Errors = new Errors<Branch>();
    public BranchServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Branch>> getBranchList()
    {
        MainResponse<Branch> response = new MainResponse<Branch>();
        var branch = await _unitOfWork.Branchs.GetAll();
        response.acceptedObjects = branch.ToList();
        return response;
    }
    public async Task<MainResponse<Branch>> getBranch(int id)
    {
        MainResponse<Branch> response = new MainResponse<Branch>();
        var branch = await _unitOfWork.Branchs.GetById(id);

        if (branch == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Branch> { branch };
        return response;
    }
    public async Task<MainResponse<Branch>> updateBranch(int id, List<BranchDTO> branchUpdated)
    {
        var response = new MainResponse<Branch>();

        try
        {
            var validList = await ValidateDTO.BranchDTO(branchUpdated , true);

            var existingBranch = await _unitOfWork.Branchs.GetFirst(a => a.Id == id);
            if (existingBranch is null)
            {
                response.errors.Add($"Cannot find Branch with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Branch>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Branch. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Branch>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingBranch);

            await _unitOfWork.Branchs.Update(existingBranch);

            response.acceptedObjects.Add(existingBranch);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Branch>>(validList.rejectedObjects));
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

    public async Task<MainResponse<Branch>> addBranch(List<BranchDTO> branch)
    {
        MainResponse<Branch> response = new MainResponse<Branch>();
        try
        {
            var validList = await ValidateDTO.BranchDTO(branch);

            List<Branch> branchlist = _mapper.Map<List<Branch>>(validList.acceptedObjects);
            List<Branch> rejectedbranch = _mapper.Map<List<Branch>>(validList.rejectedObjects);
            if (branchlist != null && branchlist.Count() > 0)
            {
                var branchlists = await _unitOfWork.Branchs.Add(branchlist);
                response.acceptedObjects = branchlist;
            }
            if (rejectedbranch != null && rejectedbranch.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedbranch;
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
    public async Task<MainResponse<Branch>> deleteBranch(int id)
    {
        MainResponse<Branch> response = new MainResponse<Branch>();

        var branch = await _unitOfWork.Branchs.DeletePhysical(p => p.Id == id);

        if (branch == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Branch> { branch.First() };
        return response;
    }
}
