using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Services
{
    public class LeadServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<User> Errors = new Errors<User>();
        public LeadServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
       
        public byte[] GenerateExcelTemplate()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Leads");

                // Headers
                var headers = new List<string>
{
    "CompanyName",
    "Name",
    "Category",
    "PhoneNo",
    "Email",
    "Status",
    "Country",
    "Notes", //8
    "DueDate",//
    "FeedBack",//10
    "Website",//11
    "Source",//12
    "CreatedAt",
    "LastEdited",//14
    "Sector",
    "PiplineStage",
    "Note",
    "Probability",
    "Channel",//19
    "EstValue",
    "Services",//21
    "FounderAcc",
    "NextFollowUp",

};

                // Add headers
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // 🎨 Styling
                using (var range = worksheet.Cells[1, 1, 1, headers.Count])
                {
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // 📏 Auto fit columns
                worksheet.Cells.AutoFitColumns();

                var statusValues = new List<string>
{
    "Cancel",
    "NotInterested",
    "Interested",
    "NotResponding",
    "Responding",
    "FollowUp",
    "Duplicated",
    "NoAction"
};
                var Coutnry = new List<string> { "EG", "KSA" };

                // 🧠 Sample row (VERY IMPORTANT FOR USERS)
                worksheet.Cells[2, 1].Value = "DemoCompany";
                worksheet.Cells[2, 2].Value = "DemoLead";
                worksheet.Cells[2, 3].Value = "manifucture";
                worksheet.Cells[2, 4].Value = "01024455000";
                worksheet.Cells[2, 5].Value = "body@gmail.com"; // AssignedTo (UserId)
                worksheet.Cells[2, 6].Value = "Status";

                // Dropdown for rows 2 → 100
                var validation = worksheet.DataValidations.AddListValidation("F2:F100");

                foreach (var value in statusValues)
                {
                    validation.Formula.Values.Add(value);
                }

                // Optional validation behavior
                validation.ShowErrorMessage = true;
                worksheet.Cells[2, 7].Value = "Country";

                // Dropdown for rows 2 → 100
                var validations = worksheet.DataValidations.AddListValidation("G2:G100");

                foreach (var value in Coutnry)
                {
                    validations.Formula.Values.Add(value);
                }
                worksheet.Cells[2, 8].Value = "Important client";
                worksheet.Cells[2, 9].Value = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                worksheet.Cells[2, 10].Value = "Waiting for response";
                worksheet.Cells[2, 11].Value = "www.abc.com";
                worksheet.Cells[2, 12].Value = "Facebook"; // Source
                worksheet.Cells[2, 13].Value = DateTime.Now.ToString("yyyy-MM-dd");
                worksheet.Cells[2, 14].Value = DateTime.Now.ToString("yyyy-MM-dd");
                worksheet.Cells[2, 15].Value = "Manufacturing";
                worksheet.Cells[2, 16].Value = "Negotiation";
                worksheet.Cells[2, 17].Value = "Client is interested";
                worksheet.Cells[2, 18].Value = "70%";
                worksheet.Cells[2, 19].Value = "Online";
                worksheet.Cells[2, 20].Value = "50000";
                worksheet.Cells[2, 21].Value = "CRM System";
                worksheet.Cells[2, 22].Value = "Founder1";
                worksheet.Cells[2, 23].Value = "Hot Lead";

                return package.GetAsByteArray();
            }
        }
        
        public async Task<MainResponse<LeadStatusPercentage>> GetLeadsStatus(string UserId, DateTime dateFrom, DateTime dateTo)
        {
            MainResponse<LeadStatusPercentage> response = new MainResponse<LeadStatusPercentage>();
            List<LeadStatusPercentage> leadStatuses = new List<LeadStatusPercentage>();

            var leads = await _unitOfWork.Leads.GetAll(l => l.AssignedTo == UserId && l.DueDate >= dateFrom && l.DueDate <= dateTo.Date.AddDays(1));

            foreach (LeadStatus status in /*leads.Select(l => l.Status).Distinct()*/Enum.GetValues(typeof(LeadStatus)))
            {
                var sts = leads.Where(u => u.Status == status).ToList();
                if (!sts.Any())
                {
                    continue;
                }
                var stsPerc = (decimal)((decimal)sts.Count() / (decimal)leads.Count()) * 100;

                leadStatuses.Add(new LeadStatusPercentage { Status = status, Percentage = Math.Round(stsPerc, 2) });
            }
            

            response.acceptedObjects = leadStatuses;

            return response;
        }

        public async Task<MainResponse<LeadsCountry>> GetLeadsCountries(string UserId, DateTime dateFrom, DateTime dateTo)
        {
            MainResponse<LeadsCountry> response = new MainResponse<LeadsCountry>();
            List<LeadsCountry> LeadsCountry = new List<LeadsCountry>();

            var leads = await _unitOfWork.Leads.GetAll(l => l.AssignedTo == UserId && l.DueDate >= dateFrom && l.DueDate <= dateTo.Date.AddDays(1));

            foreach (EG_KSA country in Enum.GetValues(typeof(EG_KSA)))
            {
                var cty = leads.Where(u => u.Country == country).ToList();
                //var ctyPerc = (decimal)((decimal)cty.Count() / (decimal)leads.Count()) * 100;

                //var ctyEG = leads.Where(u => u.Country == country).ToList();
                LeadsCountry.Add(new LeadsCountry { Country = country, Percentage = (decimal)cty.Count() });
            }

            //var ctyEG = leads.Where(u => u.Country == EG_KSA.Egypt).ToList();
            //LeadsCountry.Add(new LeadsCountry { Country = EG_KSA.Egypt, Percentage = (decimal)ctyEG.Count() });

            //var ctyKSA = leads.Where(u => u.Country == EG_KSA.KSA).ToList();
            //LeadsCountry.Add(new LeadsCountry { Country = EG_KSA.KSA, Percentage = (decimal)ctyKSA.Count() });


            response.acceptedObjects = LeadsCountry;

            return response;
        }

        public async Task<MainResponse<LeadsStatusbyAssignedUser>> GetLeadsStatusByAssignedUser(List<string> allowances)
        {
            MainResponse<LeadsStatusbyAssignedUser> response = new MainResponse<LeadsStatusbyAssignedUser>();

            var allLeads = await _unitOfWork.Leads.GetAll();

            // Filter in memory (safe)
            var leads = allLeads
                .Where(l =>
                    allowances.Contains(l.CreatedBy) ||
                    allowances.Contains(l.AssignedTo))
                .ToList();

            var users = leads
                .Select(l => l.AssignedTo)
                .Distinct();

            var result = new List<LeadsStatusbyAssignedUser>();

            foreach (var user in users)
            {
                var userLeads = leads.Where(l => l.AssignedTo == user);

                result.Add(new LeadsStatusbyAssignedUser
                {
                    //Name = user,
                    //Cancel = userLeads.Count(l => l.Status == LeadStatus.Cancel),
                    //NotInterested = userLeads.Count(l => l.Status == LeadStatus.NotInterested),
                    //Interested = userLeads.Count(l => l.Status == LeadStatus.Interested),
                    //NotResponding = userLeads.Count(l => l.Status == LeadStatus.NotResponding),
                    //FollowUp = userLeads.Count(l => l.Status == LeadStatus.FollowUp),
                    Name = user,
                    Cancel = userLeads.Count(l => l.Status == LeadStatus.Cancel),
                    NotInterested = userLeads.Count(l => l.Status == LeadStatus.NotInterested),
                    Interested = userLeads.Count(l => l.Status == LeadStatus.Interested),
                    FollowUp = userLeads.Count(l => l.Status == LeadStatus.FollowUp),
                    Duplicated = userLeads.Count(l => l.Status == LeadStatus.Duplicated),
                    NotResponding = userLeads.Count(l => l.Status == LeadStatus.NotResponding),
                    Responding = userLeads.Count(l => l.Status == LeadStatus.responding),
                    NoAction = userLeads.Count(l => l.Status == LeadStatus.NoAction),
                });
            }

            response.acceptedObjects = result;
            return response;
        }

        public async Task<MainResponse<Lead>> GetAllLeads(List<string> allowedUsers)
        {
            MainResponse<Lead> response = new MainResponse<Lead>();

            if (allowedUsers == null || !allowedUsers.Any())
            {
                response.acceptedObjects = new List<Lead>();
                return response;
            }

            // Get ALL leads once
            var allLeads = await _unitOfWork.Leads.GetAll();

            // Filter in memory (safe)
            var filtered = allLeads
                .Where(l =>
                    allowedUsers.Contains(l.CreatedBy) ||
                    allowedUsers.Contains(l.AssignedTo))
                .ToList();

            if (!filtered.Any())
            {
                response.errors.Add(Errors.ObjectNotFound());
                return response;
            }

            response.acceptedObjects = filtered;

            return response;
        }

        public async Task<MainResponse<Lead>> GetLead(int id)
        {
            MainResponse<Lead> response = new MainResponse<Lead>();
            var user = await _unitOfWork.Leads.GetById(id);

            if (user == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<Lead> { user };
            return response;
        }

        public async Task<MainResponse<Lead>> GetLeadByStatus(int status)
        {
            MainResponse<Lead> response = new MainResponse<Lead>();
            var leads = await _unitOfWork.Leads.GetBy(x => ((int)x.Status) == status);

            if (leads == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = leads.ToList();
            return response;
        }

        public async Task<MainResponse<Lead>> UpdateLead(int id, LeadDTO userUpdated)
        {
            var response = new MainResponse<Lead>();

            try
            {
                var validList = await ValidateDTO.LeadDTO(userUpdated, true);
                var existingLeads = await _unitOfWork.Leads.GetById(id);
                //var existingLeads = await _unitOfWork.Leads.GetFirst(a => a.Id == id);
                if (existingLeads is null)
                {
                    response.errors?.Add($"Cannot find lead with ID {id}.");

                    if (validList.rejectedObjects?.Any() == true && validList.errors?.Any() == true)
                        response.errors?.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects?.AddRange(_mapper.Map<List<Lead>>(validList.rejectedObjects));

                    return response;
                }

                if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
                {
                    response.errors?.Add("No valid payload to update this Lead. Fix validation errors and try again.");

                    if (validList.errors?.Any() == true)
                        response.errors?.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects?.AddRange(_mapper.Map<List<Lead>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                // Detect status change BEFORE mapping
                bool statusChanged = dto.Status != existingLeads.Status;

                // Map new values
                _mapper.Map(dto, existingLeads);

                // Business rule: update LastEdited only if status changed
                if (statusChanged)
                {
                    existingLeads.LastEdited = DateTime.Now;
                }

                // Keep your existing logic
                existingLeads.CreatedBy = existingLeads.CreatedBy;

                await _unitOfWork.Leads.Update(existingLeads);

                response.acceptedObjects ??= new List<Lead>();
                response.acceptedObjects.Add(existingLeads);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects?.AddRange(_mapper.Map<List<Lead>>(validList.rejectedObjects));

                if (validList.errors?.Any() == true)
                    response.errors?.AddRange(validList.errors);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
                if (ex.InnerException != null)
                    response.errors?.Add(ex.InnerException.Message);
            }

            return response;
        }

        public async Task<MainResponse<Lead>> AddLead(LeadDTO dto, string name)
        {
            MainResponse<Lead> response = new MainResponse<Lead>();
            try
            {
                var validList = await ValidateDTO.LeadDTO(dto);

                List<Lead> acceptedTicket = _mapper.Map<List<Lead>>(validList.acceptedObjects);
                List<Lead> rejectedTicket = _mapper.Map<List<Lead>>(validList.rejectedObjects);

                if (acceptedTicket != null && acceptedTicket.Count > 0)
                {
                    // 🔥 Set CreatedBy for all leads
                    foreach (var lead in acceptedTicket)
                    {
                        lead.CreatedBy = name;
                    }

                    await _unitOfWork.Leads.Add(acceptedTicket);

                    response.acceptedObjects = acceptedTicket;
                }
                if (rejectedTicket != null && rejectedTicket.Count() > 0)
                {
                    List<String> err = validList.errors;
                    response.rejectedObjects = rejectedTicket;
                    response.errors = err;
                }
                return response;
            }

            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { ex.Message };
                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);
                return response;
            }
        }

        public async Task<MainResponse<Lead>> DeleteLead(int id)
        {
            MainResponse<Lead> response = new MainResponse<Lead>();

            var user = await _unitOfWork.Leads.DeletePhysical(p => p.Id == id);

            if (user == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }
            response.acceptedObjects = new List<Lead> { user.First() };
            return response;
        }

        public async Task<MainResponse<Lead>> ImportFromExcel(IFormFile excelFile, string CreatedBy)
        {
            var response = new MainResponse<Lead>();

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    response.errors?.Add("Excel file is empty.");
                    return response;
                }

                using var ms = new MemoryStream();
                await excelFile.CopyToAsync(ms);
                ms.Position = 0;

                using var package = new ExcelPackage(ms);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    response.errors?.Add("Worksheet not found.");
                    return response;
                }

                int rows = worksheet.Dimension?.Rows ?? 0;
                int cols = worksheet.Dimension?.Columns ?? 0;

                if (rows < 2)
                {
                    response.errors?.Add("No data rows found.");
                    return response;
                }

                //Read Headers
                var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                for (int c = 1; c <= cols; c++)
                {
                    var header = worksheet.Cells[1, c].Text?.Trim();

                    if (!string.IsNullOrWhiteSpace(header) && !columnMap.ContainsKey(header))
                        columnMap.Add(header, c);
                }

                string GetValue(int row, string columnName)
                {
                    if (!columnMap.ContainsKey(columnName))
                        return string.Empty;

                    return worksheet.Cells[row, columnMap[columnName]].Text?.Trim();
                }

                var leads = new List<Lead>();

                for (int r = 2; r <= rows; r++)
                {
                    var name = GetValue(r, "Name");

                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    var companyName = GetValue(r, "CompanyName");
                    var category = GetValue(r, "Category");
                    var phoneNo = GetValue(r, "PhoneNo");
                    var email = GetValue(r, "Email");
                    var statusText = GetValue(r, "Status");
                    var countryText = GetValue(r, "Country");
                    var notes = GetValue(r, "Notes");
                    var dueDateText = GetValue(r, "DueDate");
                    var feedback = GetValue(r, "FeedBack");
                    var website = GetValue(r, "Website");
                    var source = GetValue(r, "Source");
                    var createdAtText = GetValue(r, "CreatedAt");
                    var lastEditedText = GetValue(r, "LastEdited");
                    var sector = GetValue(r, "Sector");
                    var piplineStage = GetValue(r, "PiplineStage");
                    var note = GetValue(r, "Note");
                    var probability = GetValue(r, "Probability");
                    var channel = GetValue(r, "Channel");
                    var estValueText = GetValue(r, "EstValue");
                    var services = GetValue(r, "Services");
                    var founderAcc = GetValue(r, "FounderAcc");
                    var nextFollowUpText = GetValue(r, "NextFollowUp");

                    Enum.TryParse(countryText, true, out EG_KSA country);

                    DateTime? dueDate = null;
                    if (DateTime.TryParse(dueDateText, out DateTime parsedDueDate))
                        dueDate = parsedDueDate;

                    DateTime createdAt = DateTime.Now;
                    if (DateTime.TryParse(createdAtText, out DateTime parsedCreatedAt))
                        createdAt = parsedCreatedAt;

                    DateTime? lastEdited = null;
                    if (DateTime.TryParse(lastEditedText, out DateTime parsedLastEdited))
                        lastEdited = parsedLastEdited;

                    DateTime? nextFollowUp = null;
                    if (DateTime.TryParse(nextFollowUpText, out DateTime parsedNextFollowUp))
                        nextFollowUp = parsedNextFollowUp;

                    decimal? estValue = null;
                    if (decimal.TryParse(estValueText, out decimal parsedEstValue))
                        estValue = parsedEstValue;

                    var dto = new LeadDTO
                    {
                        Name = name,
                        CompanyName = companyName,
                        Category = category,
                        PhoneNo = phoneNo,
                        Email = email,
                        AssignedTo = CreatedBy,
                        Status = ParseLeadStatus(statusText),
                        Country = country,
                        Notes = notes,
                        DueDate = dueDate,
                        FeedBack = feedback,
                        Website = website,
                        Source = source,
                        Sector = sector,
                        PiplineStage = piplineStage,
                        Note = note,
                        Probability = probability,
                        Channel = channel,
                        EstValue = estValue.ToString(),
                        Services = services,
                        FounderAcc = founderAcc,
                        NextFollowUp = nextFollowUp.ToString()
                    };

                    var lead = _mapper.Map<Lead>(dto);

                    lead.CreatedBy = CreatedBy;
                    lead.CreatedAt = createdAt;
                    lead.LastEdited = lastEdited;

                    leads.Add(lead);
                }

                if (!leads.Any())
                {
                    response.errors?.Add("No valid rows found.");
                    return response;
                }

                await _unitOfWork.Leads.Add(leads);

                response.acceptedObjects = leads;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors?.Add(ex.InnerException.Message);
            }

            return response;
        }
        public async Task<MainResponse<Lead>> GetRespondingLeads(string Name)
        {
            MainResponse<Lead> response = new MainResponse<Lead>();
            var leads = await _unitOfWork.Leads.GetBy(x => ((int)x.Status) == 3 && x.CreatedBy == Name);

            if (leads == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = leads.ToList();
            return response;
        }

        public async Task<MainResponse<LeadStatusCountDTO>> GetNoOfLeadsByStatus(List<string> allowedUsers)
        {
            MainResponse<LeadStatusCountDTO> response = new MainResponse<LeadStatusCountDTO>();

            try
            {
                if (allowedUsers == null || !allowedUsers.Any())
                {
                    response.acceptedObjects = new List<LeadStatusCountDTO>();
                    return response;
                }
                foreach (var x in allowedUsers)
                {
                    var leadss = _unitOfWork.Leads.GetAll(x => x.CreatedBy == x.ToString());


                }
                var allowed = allowedUsers.ToList();

                var leads = _unitOfWork.Leads.GetQueryable()
                    .AsEnumerable()
                    .Where(l => allowed.Contains(l.CreatedBy))
                    .ToList();

                var result = leads
                    .GroupBy(l => l.Status)
                    .Select(g => new LeadStatusCountDTO
                    {
                        Status = (int)g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                response.acceptedObjects = result;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<MainResponse<LeadsTodayByUserDTO>> GetLeadsTodayByUsers(string created)
        {
            MainResponse<LeadsTodayByUserDTO> response = new MainResponse<LeadsTodayByUserDTO>();

            try
            {
                var today = DateTime.Today;

                var leads = await _unitOfWork.Leads.GetAll(
                    l => l.LastEdited >= today && l.LastEdited < today.AddDays(1) && l.AssignedTo == created
                );

                var users = leads
                    .Select(l => l.CreatedBy)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct();
                var result = new LeadsTodayByUserDTO
                {
                    UserName = created,
                    Responding = leads.Count(l => l.Status == LeadStatus.responding),
                    TotalLeadsToday = leads.Count(),
                    Cancel = leads.Count(l => l.Status == LeadStatus.Cancel),
                    NotInterested = leads.Count(l => l.Status == LeadStatus.NotInterested),
                    Interested = leads.Count(l => l.Status == LeadStatus.Interested),
                    NotResponding = leads.Count(l => l.Status == LeadStatus.NotResponding),
                    FollowUp = leads.Count(l => l.Status == LeadStatus.FollowUp),
                    Duplicated = leads.Count(l => l.Status == LeadStatus.Duplicated),
                    NoAction = leads.Count(l => l.Status == LeadStatus.NoAction)
                };


                response.acceptedObjects?.Add(result);
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors?.Add(ex.Message);
            }

            return response;
        }

        private LeadStatus ParseLeadStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return LeadStatus.NotResponding;

            status = status.Trim().ToLower();

            return status switch
            {
                "cansel" => LeadStatus.Cancel,
                "not interested" => LeadStatus.NotInterested,
                "intersted" => LeadStatus.Interested,
                "مستجيب" => LeadStatus.responding,
                "follow-up" => LeadStatus.FollowUp,
                "duplicated" => LeadStatus.Duplicated,
                "not responding" => LeadStatus.NotResponding,
                "No Action" => LeadStatus.NoAction,
                _ => LeadStatus.NotResponding
            };
        }
        // DELETE GROUP
        public async Task<MainResponse<Lead>> deleteGroup(List<int> ids)
        {
            MainResponse<Lead> response = new MainResponse<Lead>();

            try
            {
                foreach (var id in ids)
                {
                    var deletedTodos = await _unitOfWork.Leads.DeletePhysical(p => p.Id == id);
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

        public async Task<MainResponse<Lead>> deleteAll()
        {
            MainResponse<Lead> response = new MainResponse<Lead>();
            try
            {
                var deletedLeads = await _unitOfWork.Leads.DeletePhysical(p => true);
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
                response?.errors?.Add(ex.Message);
            }
            return response;
        }
    }
}
