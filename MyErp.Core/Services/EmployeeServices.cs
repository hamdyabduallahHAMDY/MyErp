using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

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
    public async Task<MainResponse<Employee>> getEmployeeList()
    {
        MainResponse<Employee> response = new MainResponse<Employee>();
        var employee = await _unitOfWork.Employees.GetAll();
        response.acceptedObjects = employee.ToList();
        return response;
    }
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
    public async Task<MainResponse<Employee>> updateEmployee(int id, List<EmployeeDTO> employeeUpdated)
    {
        var response = new MainResponse<Employee>();

        try
        {
            var validList = await ValidateDTO.EmployeeDTO(employeeUpdated , true);

            var existingEmployee = await _unitOfWork.Employees.GetFirst(a => a.Id == id);
            if (existingEmployee is null)
            {
                response.errors.Add($"Cannot find Employee with ID {id}.");
                if (validList.rejectedObjects?.Any() == true)
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                response.rejectedObjects.AddRange(_mapper.Map<List<Employee>>(validList.rejectedObjects));
                return response;
            }

            if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Employee. Fix validation errors and try again.");
                if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
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
            if (ex.InnerException != null) response.errors.Add(ex.InnerException.Message);
        }

        return response;
    }

    public async Task<MainResponse<Employee>> addEmployee(List<EmployeeDTO> employee)
    {
        MainResponse<Employee> response = new MainResponse<Employee>();
        try
        {
            var validList = await ValidateDTO.EmployeeDTO(employee);

            List<Employee> employeelist = _mapper.Map<List<Employee>>(validList.acceptedObjects);
            List<Employee> rejectedemployee = _mapper.Map<List<Employee>>(validList.rejectedObjects);
            if (employeelist != null && employeelist.Count() > 0)
            {
                var employeelists = await _unitOfWork.Employees.Add(employeelist);
                response.acceptedObjects = employeelist;
            }
            if (rejectedemployee != null && rejectedemployee.Count() > 0)
            {
                List<String> err = (validList.errors);
                response.rejectedObjects = rejectedemployee;
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
    public async Task<MainResponse<Employee>> deleteEmployee(int id)
    {
        MainResponse<Employee> response = new MainResponse<Employee>();

        var employee = await _unitOfWork.Employees.DeletePhysical(p => p.Id == id);

        if (employee == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }
        response.acceptedObjects = new List<Employee> { employee.First() };
        return response;
    }
}
