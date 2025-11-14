using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class OrdermeServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Orderme> Errors = new Errors<Orderme>();
    public OrdermeServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<MainResponse<Orderme>> getOrdermeList()
    {
        MainResponse<Orderme> response = new MainResponse<Orderme>();
        var orderme = await _unitOfWork.Ordermes.GetAll();
        response.acceptedObjects = orderme.ToList();
        return response;
    }
    public async Task<MainResponse<Orderme>> getOrderme(int id)
    {
        MainResponse<Orderme> response = new MainResponse<Orderme>();
        var orderme = await _unitOfWork.Ordermes.GetById(id);

        if (orderme == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Orderme> { orderme };
        return response;
    }
    public async Task<MainResponse<Orderme>> updateOrderme(int id, List<OrdermeDTO> ordermeUpdated)
    {
        var response = new MainResponse<Orderme>();

        try
        {
            var validList = await ValidateDTO.OrdermeDTO(ordermeUpdated , true);

            var existingOrderme = await _unitOfWork.Ordermes.GetFirst(a => a.Id == id);
            if (existingOrderme is null)
            {
                response.errors.Add($"Cannot find Orderme with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Orderme>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Orderme. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Orderme>>(validList.rejectedObjects));
                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingOrderme);

            await _unitOfWork.Ordermes.Update(existingOrderme);

            response.acceptedObjects.Add(existingOrderme);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(_mapper.Map<List<Orderme>>(validList.rejectedObjects));
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

    public async Task<MainResponse<Orderme>> addOrderme(List<OrdermeDTO> orderme)
    {
        MainResponse<Orderme> response = new MainResponse<Orderme>();
        try
        {
            var validList = await ValidateDTO.OrdermeDTO(orderme);

            List<Orderme> ordermelist = _mapper.Map<List<Orderme>>(validList.acceptedObjects);
            List<Orderme> rejectedorderme = _mapper.Map<List<Orderme>>(validList.rejectedObjects);
            if (ordermelist != null && ordermelist.Count() > 0)
            {
                var ordermelists = await _unitOfWork.Ordermes.Add(ordermelist);
                response.acceptedObjects = ordermelist;
            }
            if (rejectedorderme != null && rejectedorderme.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedorderme;
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
    public async Task<MainResponse<Orderme>> deleteOrderme(int id)
    {
        MainResponse<Orderme> response = new MainResponse<Orderme>();

        var orderme = await _unitOfWork.Ordermes.DeletePhysical(p => p.Id == id);

        if (orderme == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Orderme> { orderme.First() };
        return response;
    }
}
