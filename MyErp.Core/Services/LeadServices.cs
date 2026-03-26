using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Http;
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

            //  Safety check
            if (allowedUsers == null || !allowedUsers.Any())
            {
                response.acceptedObjects = new List<Lead>();
                return response;
            }

            //  Force in-memory list (avoid EF translation issues)
            var allowed = allowedUsers.ToList();

            //  Get IQueryable
            var query = _unitOfWork.Leads.GetQueryable();

            //  Execute in DB first → then filter in memory
            var leads = query
                .AsEnumerable()
                .Where(l => allowed.Contains(l.CreatedBy))
                .ToList();

            // 🔄 Map to DTO
            var leadsDTO = _mapper.Map<List<Lead>>(leads);

            // 📤 Assign response
            response.acceptedObjects = leadsDTO;

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
        public async Task<MainResponse<Lead>> AddLead(LeadDTO dto, string name )
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
    }
}
