using AutoMapper;
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
using System.Threading.Tasks;

namespace MyErp.Core.Services
{
    public class EmployeeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<Employee> Errors = new Errors<Employee>();

        public EmployeeServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ================= GET ALL =================
        public async Task<MainResponse<Employee>> getEmployeesList()
        {
            MainResponse<Employee> response = new MainResponse<Employee>();

            var employees = await _unitOfWork.Employees.GetAll();
            response.acceptedObjects = employees.ToList();

            return response;
        }

        // ================= GET BY ID =================
        public async Task<MainResponse<Employee>> getEmployee(int id)
        {
            MainResponse<Employee> response = new MainResponse<Employee>();

            var employee = await _unitOfWork.Employees.GetById(id);

            if (employee == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<Employee> { employee };
            return response;
        }

        // ================= ADD =================
        public async Task<MainResponse<Employee>> AddEmployee(EmployeeDTO employees)
        {
            MainResponse<Employee> response = new MainResponse<Employee>();

            try
            {
                var validList = await ValidateDTO.EmployeeDTO(employees);

                List<Employee> accepted = _mapper.Map<List<Employee>>(validList.acceptedObjects);
                List<Employee> rejected = _mapper.Map<List<Employee>>(validList.rejectedObjects);

                if (accepted != null && accepted.Any())
                {
                    await _unitOfWork.Employees.Add(accepted);
                    response.acceptedObjects = accepted;
                }

                if (rejected != null && rejected.Any())
                {
                    response.rejectedObjects = rejected;
                    response.errors = validList.errors;
                }

                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
                return response;
            }
        }

        // ================= UPDATE =================
        public async Task<MainResponse<Employee>> updateEmployee(int id, EmployeeDTO employeeUpdated)
        {
            var response = new MainResponse<Employee>();

            try
            {
                var validList = await ValidateDTO.EmployeeDTO(employeeUpdated, true);
                var existingEmployee = await _unitOfWork.Employees.GetFirst(e => e.Id == id);

                if (existingEmployee is null)
                {
                    response.errors.Add($"Cannot find Employee with Id {id}.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<Employee>>(validList.rejectedObjects));

                    return response;
                }

                if (validList.acceptedObjects == null || !validList.acceptedObjects.Any())
                {
                    response.errors.Add("No valid payload to update Employee.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<Employee>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingEmployee);

                await _unitOfWork.Employees.Update(existingEmployee);

                response.acceptedObjects.Add(existingEmployee);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Employee>>(validList.rejectedObjects));

                if (validList.errors?.Any() == true)
                    response.errors.AddRange(validList.errors);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);
            }

            return response;
        }

        // ================= DELETE ONE =================
        public async Task<MainResponse<Employee>> deleteEmployee(int id)
        {
            MainResponse<Employee> response = new MainResponse<Employee>();

            var employee = await _unitOfWork.Employees.DeletePhysical(e => e.Id == id);

            if (employee == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<Employee> { employee.First() };
            return response;
        }

        // ================= DELETE GROUP =================
        public async Task<MainResponse<Employee>> deleteGroup(List<int> ids)
        {
            MainResponse<Employee> response = new MainResponse<Employee>();

            try
            {
                foreach (var id in ids)
                {
                    var deleted = await _unitOfWork.Employees.DeletePhysical(e => e.Id == id);

                    if (deleted == null || !deleted.Any())
                    {
                        response.errors?.Add($"id = {id} not found");
                        return response;
                    }

                    response.acceptedObjects = deleted.ToList();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<Employee>> deleteAll()
        {
            MainResponse<Employee> response = new MainResponse<Employee>();
            try
            {
                var deletedLeads = await _unitOfWork.Employees.DeletePhysical(p => true);
                if (deletedLeads == null || !deletedLeads.Any())
                {
                    response.errors?.Add($"No leads found to delete.");
                    return response;
                }
                else
                {
                    response.acceptedObjects = deletedLeads.ToList();
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
            }
            return response;
        }

    }
}