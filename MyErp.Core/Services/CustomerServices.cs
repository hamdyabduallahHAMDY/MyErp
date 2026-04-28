using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Http;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using OfficeOpenXml;
using System.Text.Json;
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
            Logs.Log($"[SYSTEM] Generating customer Excel template");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Customer Template");

            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "TaxRegistrationNumber";
            worksheet.Cells[1, 3].Value = "CompanyName";
            worksheet.Cells[1, 4].Value = "Phone";
            worksheet.Cells[1, 5].Value = "AnyDesk";
            worksheet.Cells[1, 6].Value = "POC";

            worksheet.Cells[2, 1].Value = "Ahmed Ali";
            worksheet.Cells[2, 2].Value = "123456789";
            worksheet.Cells[2, 3].Value = "Tech Solutions";
            worksheet.Cells[2, 4].Value = "01012345678";
            worksheet.Cells[2, 5].Value = "123-456-789";
            worksheet.Cells[2, 6].Value = "Mohamed Hassan";

            worksheet.Cells.AutoFitColumns();

            return await package.GetAsByteArrayAsync();
        }

        public async Task<MainResponse<Customer>> getProjectsByAccess(string currentUser)
        {
            var response = new MainResponse<Customer>();

            if (string.IsNullOrEmpty(currentUser))
            {
                Logs.Log($"[AUTH] Unauthorized access to getProjectsByAccess");
                response.errors = new List<string> { "User is not authenticated" };
                return response;
            }

            try
            {
                Logs.Log($"[USER: {currentUser}] Fetching accessible projects");

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

                Logs.Log($"[USER: {currentUser}] Found {filtered.Count} accessible projects");

                foreach (var project in projects)
                {
                    if (project.allowance != null)
                    {
                        var countUsers = JsonSerializer.Deserialize<List<string>>(project.allowance).Count();
                        project.allowance = countUsers.ToString();
                    }
                }

                response.acceptedObjects = filtered;
            }
            catch (Exception ex)
            {
                Logs.Log($"[USER: {currentUser}] Error in getProjectsByAccess: {ex}");
                response.errors = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<MainResponse<Customer>> getCustomersListByType(Type type)
        {
            Logs.Log($"[SYSTEM] Fetching customers by type {type}");

            MainResponse<Customer> response = new MainResponse<Customer>();

            var customers = await _unitOfWork.Customers
                .GetAll(x => x.Type == type);

            response.acceptedObjects = customers.ToList();

            return response;
        }

        public async Task<MainResponse<Customer>> getCustomersOdoo(Type type)
        {
            Logs.Log($"[SYSTEM] Fetching Odoo customers");

            MainResponse<Customer> response = new MainResponse<Customer>();

            var customers = await _unitOfWork.Customers
                .GetAll(x => x.Type == Type.Odoo);

            response.acceptedObjects = customers.ToList();

            return response;
        }

        public async Task<MainResponse<Customer>> getCustomer(int id)
        {
            var response = new MainResponse<Customer>();

            Logs.Log($"[SYSTEM] Fetching customer Id {id}");

            var customer = await _unitOfWork.Customers.GetById(id);

            if (customer == null)
            {
                Logs.Log($"[SYSTEM] Customer not found Id {id}");
                response.errors = new List<string> { Errors.ObjectNotFound() };
                return response;
            }

            response.acceptedObjects = new List<Customer> { customer };
            return response;
        }

        public async Task<MainResponse<Customer>> updateCustomer(int id, List<CustomerDTO> customerUpdated, string createdby)
        {
            var response = new MainResponse<Customer>();

            try
            {
                Logs.Log($"[USER: {createdby}] Updating customer Id {id}");

                var validList = await ValidateDTO.CustomerDTO(customerUpdated, true);
                var existingCustomer = await _unitOfWork.Customers.GetFirst(c => c.Id == id);

                if (existingCustomer is null)
                {
                    Logs.Log($"[USER: {createdby}] Customer not found Id {id}");
                    response.errors?.Add($"Cannot find Customer with Id {id}.");
                    return response;
                }

                if (validList.acceptedObjects == null || validList.acceptedObjects.Count == 0)
                {
                    Logs.Log($"[USER: {createdby}] No valid payload for update Id {id}");
                    response.errors?.Add("No valid payload to update Customer.");
                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingCustomer);

                existingCustomer.Type = dto.Type;
                existingCustomer.CreatedBy = createdby;

                await _unitOfWork.Customers.Update(existingCustomer);

                Logs.Log($"[USER: {createdby}] Customer updated Id {id}");

                response.acceptedObjects?.Add(existingCustomer);
            }
            catch (Exception ex)
            {
                Logs.Log($"[USER: {createdby}] Error updating customer Id {id}: {ex}");
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<Customer>> ImportFromExcel(IFormFile excelFile, string currentuser)
        {
            var response = new MainResponse<Customer>();

            try
            {
                Logs.Log($"[USER: {currentuser}] Importing customers from Excel");

                if (excelFile == null || excelFile.Length == 0)
                {
                    Logs.Log($"[USER: {currentuser}] Empty Excel file");
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
                    Logs.Log($"[USER: {currentuser}] Worksheet not found");
                    response.errors.Add("Worksheet not found.");
                    return response;
                }

                int rows = worksheet.Dimension?.Rows ?? 0;

                for (int r = 2; r <= rows; r++)
                {
                    var name = worksheet.Cells[r, 1].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    var dto = new CustomerDTO
                    {
                        Name = name,
                        TaxRegistrationNumber = worksheet.Cells[r, 2].Text?.Trim(),
                        CompanyName = worksheet.Cells[r, 3].Text?.Trim(),
                        AnyDesk = worksheet.Cells[r, 5].Text?.Trim(),
                        Phone = worksheet.Cells[r, 4].Text?.Trim(),
                        POC = worksheet.Cells[r, 6].Text?.Trim(),
                    };

                    var document = _mapper.Map<Customer>(dto);
                    document.CreatedBy = currentuser;

                    docsToAdd.Add(document);
                }

                if (!docsToAdd.Any())
                {
                    Logs.Log($"[USER: {currentuser}] No valid rows found");
                    response.errors.Add("No valid rows found.");
                    return response;
                }

                foreach (var doc in docsToAdd)
                    await _unitOfWork.Customers.Add(doc);

                Logs.Log($"[USER: {currentuser}] Imported {docsToAdd.Count} customers");

                response.acceptedObjects = docsToAdd;
            }
            catch (Exception ex)
            {
                Logs.Log($"[USER: {currentuser}] Error importing Excel: {ex}");
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<Customer>> addCustomer(List<CustomerDTO> customer, Type type, string createdby)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();

            try
            {
                Logs.Log($"[USER: {createdby}] Adding customers count: {customer.Count}");

                var validList = await ValidateDTO.CustomerDTO(customer);

                List<Customer> Customerlist = _mapper.Map<List<Customer>>(validList.acceptedObjects);

                foreach (var cast in Customerlist)
                {
                    cast.CreatedBy = createdby;
                }

                if (Customerlist.Any())
                {
                    await _unitOfWork.Customers.Add(Customerlist);
                    Logs.Log($"[USER: {createdby}] Added {Customerlist.Count} customers");
                    response.acceptedObjects = Customerlist;
                }

                if (validList.errors?.Any() == true)
                    response.errors = validList.errors;
            }
            catch (Exception ex)
            {
                Logs.Log($"[USER: {createdby}] Error adding customers: {ex}");
            }

            return response;
        }

        public async Task<MainResponse<Customer>> deleteUser(int id)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();

            Logs.Log($"[SYSTEM] Deleting customer Id {id}");

            var customer = await _unitOfWork.Customers.Delete(p => p.Id == id);

            if (customer == null)
            {
                Logs.Log($"[SYSTEM] Delete failed, customer not found Id {id}");
                response.errors = new List<string> { Errors.ObjectNotFoundWithId(id) };
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
                Logs.Log($"[SYSTEM] Deleting group count: {ids.Count}");

                foreach (var id in ids)
                {
                    var deleted = await _unitOfWork.Customers.Delete(p => p.Id == id);

                    if (deleted == null || !deleted.Any())
                    {
                        Logs.Log($"[SYSTEM] Delete failed for Id {id}");
                        response.errors?.Add($"id = {id} not found");
                        return response;
                    }
                    else
                    {
                        response.acceptedObjects = deleted.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.Log($"[SYSTEM] Error deleting group: {ex}");
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<Customer>> deleteAll(string user)
        {
            MainResponse<Customer> response = new MainResponse<Customer>();

            try
            {
                Logs.Log($"[SYSTEM] Deleting ALL customers");

                var deleted = await _unitOfWork.Customers.Delete(p => p.CreatedBy == user);

                if (deleted == null || !deleted.Any())
                {
                    Logs.Log($"[SYSTEM] No customers found to delete");
                    response.errors?.Add($"No leads found to delete.");
                    return response;
                }

                response.acceptedObjects = deleted.ToList();
            }
            catch (Exception ex)
            {
                Logs.Log($"[SYSTEM] Error deleting all customers: {ex}");
                response.errors?.Add(ex.Message);
            }

            return response;
        }
    }
}