using AutoMapper;
using Azure;
using Logger;
using Microsoft.AspNetCore.Http;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Type = MyErp.Core.Models.Type;

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

        public async Task<byte[]> GenerateCustomerExcelTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Customer Template");

            // ================= HEADERS =================
            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "TaxRegistrationNumber";
            worksheet.Cells[1, 3].Value = "CompanyName";
            worksheet.Cells[1, 4].Value = "Phone";
            worksheet.Cells[1, 5].Value = "AnyDesk";
            worksheet.Cells[1, 6].Value = "POC";

            // ================= DEMO ROW =================
            worksheet.Cells[2, 1].Value = "Ahmed Ali";
            worksheet.Cells[2, 2].Value = "123456789";
            worksheet.Cells[2, 3].Value = "Tech Solutions";
            worksheet.Cells[2, 4].Value = "01012345678";
            worksheet.Cells[2, 5].Value = "123-456-789";
            worksheet.Cells[2, 6].Value = "Mohamed Hassan";

            // ================= STYLING =================
            using (var header = worksheet.Cells[1, 1, 1, 6])
            {
                header.Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<MainResponse<Customer>> getProjectsByAccess(string currentUser)
        {
            var response = new MainResponse<Customer>();

           // var currentUser = user.Identity?.Name;

            if (string.IsNullOrEmpty(currentUser))
            {
                response.errors = new List<string> { "User is not authenticated" };
                return response;
            }

            try
            {
                var projects = await _unitOfWork.Customers.GetAll();

               
                var filtered = projects
                    .AsEnumerable()
                    .Where(p =>
                        p.CreatedBy == currentUser ||
                        (
                            !string.IsNullOrEmpty(p.allowance) &&
                            JsonSerializer.Deserialize<List<string>>(p.allowance)
                                ?.Contains(currentUser) == true
                        )
                    )
                    .ToList();
                foreach (var project in projects)
                {
                    if (!(project.allowance == null))
                    {
                        var countUsers = JsonSerializer.Deserialize<List<string>>(project.allowance).Count();
                        project.allowance = countUsers.ToString();
                    }
                }
                response.acceptedObjects = filtered;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<MainResponse<Customer>> getCustomersListByType(Type type)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();

            var customers = await _unitOfWork.Customers
                .GetAll(x => x.Type == type);

            response.acceptedObjects = customers.ToList();

            return response;
        }

        public async Task<MainResponse<Customer>> getCustomersOdoo(Type type)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();

            var customers = await _unitOfWork.Customers
                .GetAll(x => x.Type == Type.Odoo);

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

        public async Task<MainResponse<Customer>> updateCustomer(int id, List<CustomerDTO> customerUpdated , string cretaedby)
        {
            var response = new MainResponse<Customer>();

            try
            {
                var validList = await ValidateDTO.CustomerDTO(customerUpdated, true);
                var existingCustomer = await _unitOfWork.Customers.GetFirst(c => c.Id == id);


                if (existingCustomer is null)
                {
                    response.errors?.Add($"Cannot find Customer with Id {id}.");

                    if (validList.rejectedObjects?.Any() == true)
                        if (validList.errors?.Any() == true) response.errors?.AddRange(validList.errors);
                    response.rejectedObjects?.AddRange(_mapper.Map<List<Customer>>(validList.rejectedObjects));
                    return response;
                }

                if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
                {
                    response.errors?.Add("No valid payload to update Customer. Fix validation errors and try again.");
                    if (validList.errors?.Any() == true) response.errors?.AddRange(validList.errors);
                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects?.AddRange(_mapper.Map<List<Customer>>(validList.rejectedObjects));
                    return response;
                }

                var dto = validList.acceptedObjects[0];
                
                _mapper.Map(dto, existingCustomer);

                //aaaaaaaaaaaaaaaaaaaaaaahhhhhh 
                existingCustomer.Type = dto.Type;
                existingCustomer.CreatedBy = cretaedby;
                await _unitOfWork.Customers.Update(existingCustomer);

                response.acceptedObjects?.Add(existingCustomer);

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

        public async Task<MainResponse<Customer>> ImportFromExcel(IFormFile excelFile , string currentuser)
        {
            var response = new MainResponse<Customer>();

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    response.errors.Add("Excel file is empty.");
                    return response;
                }

                var docsToAdd = new List<Customer>();

                using var ms = new MemoryStream();
                await excelFile.CopyToAsync(ms);
                ms.Position = 0;

                using var package = new ExcelPackage(ms);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    response.errors.Add("Worksheet not found.");
                    return response;
                }

                int rows = worksheet.Dimension?.Rows ?? 0;

                for (int r = 2; r <= rows; r++) // skip header
                {
                    var name = worksheet.Cells[r, 1].Text?.Trim();
                    var TaxRegistrationNumber = worksheet.Cells[r, 2].Text?.Trim();
                    var CompanyName = worksheet.Cells[r, 3].Text?.Trim();
                    var Phone = worksheet.Cells[r, 4].Text?.Trim();
                    var AnyDesk = worksheet.Cells[r, 5].Text?.Trim();
                    var POC = worksheet.Cells[r, 6].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    //  DTO creation
                    var dto = new CustomerDTO
                    {
                        Name = name,
                        TaxRegistrationNumber = TaxRegistrationNumber,
                        CompanyName = CompanyName,
                        AnyDesk = AnyDesk,
                        Phone = Phone,
                        POC = POC,
                        
                    };

                    //  AutoMapper mapping
                    var document = _mapper.Map<Customer>(dto);
                    document.CreatedBy = currentuser;
                    // extra safety
                    //document.Attachment = null;

                    docsToAdd.Add(document);
                }

                if (!docsToAdd.Any())
                {
                    response.errors.Add("No valid rows found.");
                    return response;
                }

                foreach (var doc in docsToAdd)
                    await _unitOfWork.Customers.Add(doc);

                response.acceptedObjects = docsToAdd;
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

        public async Task<MainResponse<Customer>> addCustomer(List<CustomerDTO> customer , Type type , string createdby)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();
            try
            {
                var validList = await ValidateDTO.CustomerDTO(customer);

                List<Customer> Customerlist = _mapper.Map<List<Customer>>(validList.acceptedObjects);
                List<Customer> rejectedCustomer = _mapper.Map<List<Customer>>(validList.rejectedObjects);
                foreach ( var cast in customer) 
                {
                    cast.Type = type;                    
                }
                foreach ( var cast in Customerlist) 
                {
                    cast.CreatedBy = createdby;                    
                }
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

        public async Task<MainResponse<Customer>> deleteGroup(List<int> ids)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();

            try
            {
                foreach (var id in ids)
                {
                    var deletedTodos = await _unitOfWork.Customers.DeletePhysical(p => p.Id == id);
                    if (deletedTodos == null || !deletedTodos.Any())
                    {
                        response.errors?.Add($"id = {id} not found");
                        return response;
                    }
                    else
                    {
                        response.acceptedObjects = deletedTodos.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<MainResponse<Customer>> deleteAll()
        {
            MainResponse<Customer> response = new MainResponse<Customer>();
            try
            {
                var deletedLeads = await _unitOfWork.Customers.DeletePhysical(p => true);
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
