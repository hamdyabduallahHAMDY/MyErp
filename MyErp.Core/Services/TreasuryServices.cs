using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class TreasuryServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Treasury> Errors = new Errors<Treasury>();
    public TreasuryServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Treasury>> getTreasuryList()
    {
        MainResponse<Treasury> response = new MainResponse<Treasury>();
        var treasury = await _unitOfWork.Treasurys.GetAll();
        response.acceptedObjects = treasury.ToList();
        return response;
    }
    public async Task<MainResponse<Treasury>> getTreasury(int id)
    {
        MainResponse<Treasury> response = new MainResponse<Treasury>();
        var treasury = await _unitOfWork.Treasurys.GetById(id);

        if (treasury == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Treasury> { treasury };
        return response;
    }
    public async Task<MainResponse<Treasury>> updateTreasury(int id, List<TreasuryDTO> treasuryUpdated)
    {
        var response = new MainResponse<Treasury>();

        try
        {
            var validList = await ValidateDTO.TreasuryDTO(treasuryUpdated, true);

            var existingTreasury = await _unitOfWork.Treasurys.GetFirst(a => a.Id == id);
            if (existingTreasury is null)
            {
                response.errors.Add($"Cannot find Treasury with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Treasury>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Treasury. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Treasury>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingTreasury);

            await _unitOfWork.Treasurys.Update(existingTreasury);

            response.acceptedObjects.Add(existingTreasury);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Treasury>>(validList.rejectedObjects));
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

    public async Task<MainResponse<Treasury>> addTreasury(List<TreasuryDTO> treasury)
    {
        MainResponse<Treasury> response = new MainResponse<Treasury>();
        try
        {
            var validList = await ValidateDTO.TreasuryDTO(treasury);

            List<Treasury> treasurylist = _mapper.Map<List<Treasury>>(validList.acceptedObjects);
            List<Treasury> rejectedtreasury = _mapper.Map<List<Treasury>>(validList.rejectedObjects);
            if (treasurylist != null && treasurylist.Count() > 0)
            {
                var treasurylists = await _unitOfWork.Treasurys.Add(treasurylist);
                response.acceptedObjects = treasurylist;
            }
            if (rejectedtreasury != null && rejectedtreasury.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedtreasury;
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
    public async Task<MainResponse<Treasury>> deleteTreasury(int id)
    {
        MainResponse<Treasury> response = new MainResponse<Treasury>();

        var treasury = await _unitOfWork.Treasurys.DeletePhysical(p => p.Id == id);

        if (treasury == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Treasury> { treasury.First() };
        return response;
    }
    public static string GetCode(int orderType)
    {
        return orderType switch
        {
            0 => "160101", // Sale
            1 => "260101", // Purchase
            2 => "160102", // Credit Sale
            3 => "260103", // Return Sale
            4 => "160103", // Return Purchase
            5 => "260102", // Credit Purchase
            7 => "offerorPending"
        };
    }
}
