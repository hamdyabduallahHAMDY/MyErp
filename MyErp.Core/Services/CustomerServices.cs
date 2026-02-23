using AutoMapper;
using Azure;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Services
{
    public class CustomerServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<Customer> Errors = new Errors<Customer>();

        public CustomerServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MainResponse<Customer>> getCustomersList()
        {
            MainResponse<Customer> response = new MainResponse<Customer>();
            var customers = await _unitOfWork.Customers.GetAll();
            response.acceptedObjects = customers.ToList();
            return response;
        }

        public async Task<MainResponse<Customer>> getCustomer(int id)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();
            var customer = await _unitOfWork.Customers.GetById(id);
            if (customer == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }
            response.acceptedObjects = new List<Customer> { customer };
            return response;
        }

        public async Task<MainResponse<Customer>> updateCustomer(int id, List<CustomerDTO> customerUpdated)
        {
            var response = new MainResponse<Customer>();

            try
            {
                var validList = await ValidateDTO.CustomerDTO(customerUpdated, true);
                var existingCustomer = await _unitOfWork.Customers.GetFirst(c => c.Id == id);


                if (existingCustomer is null)
                {
                    response.errors.Add($"Cannot find Customer with Id {id}.");

                    if (validList.rejectedObjects?.Any() == true)
                        if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                    response.rejectedObjects.AddRange(_mapper.Map<List<Customer>>(validList.rejectedObjects));
                    return response;
                }

                if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
                {
                    response.errors.Add("No valid payload to update Customer. Fix validation errors and try again.");
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<Customer>>(validList.rejectedObjects));
                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingCustomer);

                await _unitOfWork.Customers.Update(existingCustomer);

                response.acceptedObjects.Add(existingCustomer);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Customer>>(validList.rejectedObjects));
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

        public async Task<MainResponse<Customer>> addCustomer(List<CustomerDTO> customer)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();
            try
            {
                var validList = await ValidateDTO.CustomerDTO(customer);

                List<Customer> Customerlist = _mapper.Map<List<Customer>>(validList.acceptedObjects);
                List<Customer> rejectedCustomer = _mapper.Map<List<Customer>>(validList.rejectedObjects);
                if (Customerlist != null && Customerlist.Count() > 0)
                {
                    var customerlists = await _unitOfWork.Customers.Add(Customerlist);
                    response.acceptedObjects = Customerlist;
                }
                if (rejectedCustomer != null && rejectedCustomer.Count() > 0)
                {
                    List<String> err = (validList.errors);
                    response.rejectedObjects = rejectedCustomer;
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

        public async Task<MainResponse<Customer>> deleteUser(int id)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();

            var customer = await _unitOfWork.Customers.DeletePhysical(p => p.Id == id);

            if (customer == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }
            response.acceptedObjects = new List<Customer> { customer.First() };
            return response;
        }


    }
}
