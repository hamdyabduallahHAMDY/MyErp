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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyErp.Core.Services
{
    public class EmailServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        Errors<Email> Errors = new Errors<Email>();

        public EmailServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET ALL EMAILS
        public async Task<MainResponse<Email>> getEmailsList()
        {
            MainResponse<Email> response = new MainResponse<Email>();

            var emails = await _unitOfWork.Emails.GetAll();

            response.acceptedObjects = emails.ToList();

            return response;
        }

        // GET EMAIL BY ID
        public async Task<MainResponse<Email>> getEmail(int id)
        {
            MainResponse<Email> response = new MainResponse<Email>();

            var email = await _unitOfWork.Emails.GetById(id);

            if (email == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<Email> { email };

            return response;
        }

        // ADD EMAIL
        public async Task<MainResponse<Email>> addEmail(EmailDTO emailDTOs)
        {
            MainResponse<Email> response = new MainResponse<Email>();

            try
            {
                var validList = await ValidateDTO.EmailDTO(emailDTOs);

                List<Email> emailList =
                    _mapper.Map<List<Email>>(validList.acceptedObjects);

                List<Email> rejectedEmails =
                    _mapper.Map<List<Email>>(validList.rejectedObjects);

                if (emailList != null && emailList.Count > 0)
                {
                    await _unitOfWork.Emails.Add(emailList);

                    response.acceptedObjects = emailList;
                }

                if (rejectedEmails != null && rejectedEmails.Count > 0)
                {
                    response.rejectedObjects = rejectedEmails;
                    response.errors = validList.errors;
                }

                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());

                response.errors?.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors?.Add(ex.InnerException.Message);

                return response;
            }
        }

        // IMPORT EMAILS FROM EXCEL
        public async Task<MainResponse<Email>> ImportFromExcel(IFormFile excelFile)
        {
            var response = new MainResponse<Email>();

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    response.errors?.Add("Excel file is empty.");
                    return response;
                }

                var emailsToAdd = new List<Email>();

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

                for (int r = 2; r <= rows; r++)
                {
                   // var emailAddress = worksheet.Cells[r, 1].Text?.Trim();
                    var subject = worksheet.Cells[r, 2].Text?.Trim();
                    var body = worksheet.Cells[r, 3].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(subject))
                        continue;

                    var dto = new EmailDTO
                    {
                        
                        Subject = subject,
                        Body = body
                    };

                    var email = _mapper.Map<Email>(dto);

                    emailsToAdd.Add(email);
                }

                if (!emailsToAdd.Any())
                {
                    response.errors?.Add("No valid rows found.");
                    return response;
                }

                foreach (var email in emailsToAdd)
                    await _unitOfWork.Emails.Add(email);

                response.acceptedObjects = emailsToAdd;
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

        // UPDATE EMAIL
        public async Task<MainResponse<Email>> updateEmail(int id, EmailDTO emailUpdated)
        {
            var response = new MainResponse<Email>();

            try
            {
                var validList = await ValidateDTO.EmailDTO(emailUpdated, true);

                var existingEmail =
                    await _unitOfWork.Emails.GetFirst(e => e.Id == id);

                if (existingEmail is null)
                {
                    response.errors?.Add($"Cannot find Email with Id {id}.");

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects?.AddRange(
                            _mapper.Map<List<Email>>(validList.rejectedObjects));

                    if (validList.errors?.Any() == true)
                        response.errors?.AddRange(validList.errors);

                    return response;
                }

                if (validList.acceptedObjects == null ||
                    validList.acceptedObjects.Count == 0)
                {
                    response.errors?.Add("No valid payload to update Email.");

                    if (validList.errors?.Any() == true)
                        response.errors?.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects?.AddRange(
                            _mapper.Map<List<Email>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingEmail);

                await _unitOfWork.Emails.Update(existingEmail);

                response.acceptedObjects?.Add(existingEmail);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects?.AddRange(
                        _mapper.Map<List<Email>>(validList.rejectedObjects));

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

        // DELETE EMAIL
        public async Task<MainResponse<Email>> deleteEmail(int id)
        {
            MainResponse<Email> response = new MainResponse<Email>();

            var email =
                await _unitOfWork.Emails.DeletePhysical(e => e.Id == id);

            if (email == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);

                response.errors = new List<string> { error };

                return response;
            }

            response.acceptedObjects = new List<Email> { email.First() };

            return response;
        }
    }
}