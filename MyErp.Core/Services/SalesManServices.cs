using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class SalesManServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<SalesMan> Errors = new Errors<SalesMan>();
    public SalesManServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<SalesMan>> getSalesManList()
    {
        MainResponse<SalesMan> response = new MainResponse<SalesMan>();
        var salesMan = await _unitOfWork.SalesMen.GetAll();
        response.acceptedObjects = salesMan.ToList();
        return response;
    }
    public async Task<MainResponse<SalesMan>> getSalesMan(int id)
    {
        MainResponse<SalesMan> response = new MainResponse<SalesMan>();
        var salesMan = await _unitOfWork.SalesMen.GetById(id);

        if (salesMan == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<SalesMan> { salesMan };
        return response;
    }
    public async Task<MainResponse<SalesMan>> updateSalesMan(int id, List<SalesManDTO> salesManUpdated)
    {
        var response = new MainResponse<SalesMan>();

        try
        {
            var validList = await ValidateDTO.SalesManDTO(salesManUpdated , true);

            var existingSalesMan = await _unitOfWork.SalesMen.GetFirst(a => a.Id == id);
            if (existingSalesMan is null)
            {
                response.errors.Add($"Cannot find SalesMan with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<SalesMan>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update SalesMan. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<SalesMan>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingSalesMan);

            await _unitOfWork.SalesMen.Update(existingSalesMan);

            response.acceptedObjects.Add(existingSalesMan);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<SalesMan>>(validList.rejectedObjects));
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

    public async Task<MainResponse<SalesMan>> addSalesMan(List<SalesManDTO> salesMan)
    {
        MainResponse<SalesMan> response = new MainResponse<SalesMan>();
        try
        {
            var validList = await ValidateDTO.SalesManDTO(salesMan);

            List<SalesMan> salesManlist = _mapper.Map<List<SalesMan>>(validList.acceptedObjects);
            List<SalesMan> rejectedsalesMan = _mapper.Map<List<SalesMan>>(validList.rejectedObjects);
            if (salesManlist != null && salesManlist.Count() > 0)
            {
                var salesManlists = await _unitOfWork.SalesMen.Add(salesManlist);
                response.acceptedObjects = salesManlist;
            }
            if (rejectedsalesMan != null && rejectedsalesMan.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedsalesMan;
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
    public async Task<MainResponse<SalesMan>> deleteSalesMan(int id)
    {
        MainResponse<SalesMan> response = new MainResponse<SalesMan>();

        var salesMan = await _unitOfWork.SalesMen.DeletePhysical(p => p.Id == id);

        if (salesMan == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<SalesMan> { salesMan.First() };
        return response;
    }
}
