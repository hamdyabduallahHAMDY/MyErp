using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class OrdermedetailServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Ordermedetail> Errors = new Errors<Ordermedetail>();
    public OrdermedetailServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Ordermedetail>> getOrdermedetailList()
    {
        MainResponse<Ordermedetail> response = new MainResponse<Ordermedetail>();
        var ordermedetail = await _unitOfWork.Ordermedetails.GetAll();
        response.acceptedObjects = ordermedetail.ToList();
        return response;
    }
    public async Task<MainResponse<Ordermedetail>> getOrdermedetail(int id)
    {
        MainResponse<Ordermedetail> response = new MainResponse<Ordermedetail>();
        var ordermedetail = await _unitOfWork.Ordermedetails.GetById(id);

        if (ordermedetail == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Ordermedetail> { ordermedetail };
        return response;
    }
    public async Task<MainResponse<Ordermedetail>> updateOrdermedetail(int id, List<OrdermedetailDTO> ordermedetailUpdated)
    {
        var response = new MainResponse<Ordermedetail>();

        try
        {
            var validList = await ValidateDTO.OrdermedetailDTO(ordermedetailUpdated , true);

            var existingOrdermedetail = await _unitOfWork.Ordermedetails.GetFirst(a => a.Id == id);
            if (existingOrdermedetail is null)
            {
                response.errors.Add($"Cannot find Ordermedetail with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Ordermedetail>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Ordermedetail. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Ordermedetail>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingOrdermedetail);

            await _unitOfWork.Ordermedetails.Update(existingOrdermedetail);

            response.acceptedObjects.Add(existingOrdermedetail);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Ordermedetail>>(validList.rejectedObjects));
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

    public async Task<MainResponse<Ordermedetail>> addOrdermedetail(List<OrdermedetailDTO> ordermedetail)
    {
        MainResponse<Ordermedetail> response = new MainResponse<Ordermedetail>();
        try
        {
            var validList = await ValidateDTO.OrdermedetailDTO(ordermedetail);

            List<Ordermedetail> ordermedetaillist = _mapper.Map<List<Ordermedetail>>(validList.acceptedObjects);
            List<Ordermedetail> rejectedordermedetail = _mapper.Map<List<Ordermedetail>>(validList.rejectedObjects);
            if (ordermedetaillist != null && ordermedetaillist.Count() > 0)
            {
                var ordermedetaillists = await _unitOfWork.Ordermedetails.Add(ordermedetaillist);
                response.acceptedObjects = ordermedetaillist;
            }
            if (rejectedordermedetail != null && rejectedordermedetail.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedordermedetail;
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
    public async Task<MainResponse<Ordermedetail>> deleteOrdermedetail(int id)
    {
        MainResponse<Ordermedetail> response = new MainResponse<Ordermedetail>();

        var ordermedetail = await _unitOfWork.Ordermedetails.DeletePhysical(p => p.Id == id);

        if (ordermedetail == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Ordermedetail> { ordermedetail.First() };
        return response;
    }
}
