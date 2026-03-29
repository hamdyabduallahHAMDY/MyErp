using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Http;
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

                // 🎯 Headers
                var headers = new List<string>
        {
            "Name",
            "PhoneNo",
            "Email",
            "CompanyName",
            "AssignedTo",
            "Status",
            "Country",
            "Notes",
            "DueDate",
            "FeedBack",
            "Website"
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

                // 🧠 Optional: Add sample row (VERY useful for users)
                worksheet.Cells[2, 1].Value = "John Doe";
                worksheet.Cells[2, 2].Value = "01000000000";
                worksheet.Cells[2, 3].Value = "john@email.com";
                worksheet.Cells[2, 4].Value = "ABC Company";
                worksheet.Cells[2, 5].Value = "ahmed";
                worksheet.Cells[2, 6].Value = "follow-up"; // Status
                worksheet.Cells[2, 7].Value = "EG";
                worksheet.Cells[2, 8].Value = "Important client";
                worksheet.Cells[2, 9].Value = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                worksheet.Cells[2, 10].Value = "Follow up";
                worksheet.Cells[2, 11].Value = "www.abc.com";

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

            var leads = await _unitOfWork.Leads.GetAll(l=>l.AssignedTo == UserId && l.DueDate >= dateFrom && l.DueDate <= dateTo.Date.AddDays(1));

            foreach(EG_KSA country in Enum.GetValues(typeof(EG_KSA)))
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
        public async Task<MainResponse<Lead>> UpdateLead(int id, LeadDTO userUpdated , string createdby)



        public async Task<MainResponse<Lead>> UpdateLead(int id, LeadDTO userUpdated)
        {
            var response = new MainResponse<Lead>();

            try
            {
                var validList = await ValidateDTO.LeadDTO(userUpdated, true);

                var existingLeads = await _unitOfWork.Leads.GetFirst(a => a.Id == id);
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
                    if (validList.errors?.Any() == true) response.errors?.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects?.AddRange(_mapper.Map<List<Lead>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];
               
                _mapper.Map(dto, existingLeads);
                existingLeads.CreatedBy = createdby;
                
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
                if (ex.InnerException != null) response.errors?.Add(ex.InnerException.Message);
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
        public async Task<MainResponse<Lead>> ImportFromExcel(IFormFile excelFile)
        {
            var response = new MainResponse<Lead>();

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    response.errors?.Add("Excel file is empty.");
                    return response;
                }

                var todosToAdd = new List<Lead>();

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
                var Leads = new List<Lead>();
                int rows = worksheet.Dimension?.Rows ?? 0;

                for (int r = 2; r <= rows; r++)
                {
                    var name = worksheet.Cells[r, 1].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    var phone = worksheet.Cells[r, 2].Text?.Trim();
                    var email = worksheet.Cells[r, 3].Text?.Trim();
                    var company = worksheet.Cells[r, 4].Text?.Trim();
                    var statusText = worksheet.Cells[r, 5].Text?.Trim();
                    var owner = worksheet.Cells[r, 6].Text?.Trim();
                    var countryText = worksheet.Cells[r, 7].Text?.Trim();
                    var notes = worksheet.Cells[r, 8].Text?.Trim();
                    var dueDateText = worksheet.Cells[r, 9].Text?.Trim();
                    var feedback = worksheet.Cells[r, 10].Text?.Trim();
                    var Website = worksheet.Cells[r, 11].Text?.Trim();

                    Enum.TryParse(statusText, true, out LeadStatus status);
                    Enum.TryParse(countryText, true, out EG_KSA country);

                    DateTime? dueDate = null;
                    if (DateTime.TryParse(dueDateText, out DateTime parsedDate))
                        dueDate = parsedDate;


                    var dto = new LeadDTO
                    {
                        Name = owner,
                        PhoneNo = phone,
                        Email = email,
                        CompanyName = company,
                        AssignedTo = name,
                        Status = ParseLeadStatus(statusText),
                        Country = country,
                        Notes = notes,
                        DueDate = dueDate,
                        FeedBack = feedback
                    };
                    var lead = _mapper.Map<Lead>(dto);

                    Leads.Add(lead);
                }

                if (!Leads.Any())
                {
                    response.errors?.Add("No valid rows found.");
                    return response;
                }

                await _unitOfWork.Leads.Add(Leads);

                response.acceptedObjects = Leads;
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
    }
}
