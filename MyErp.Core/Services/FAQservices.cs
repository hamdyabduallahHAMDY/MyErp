using AutoMapper;
using Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using System.Threading.Tasks;

namespace MyErp.Core.Services
{
    public class FAQServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<FAQ> Errors = new Errors<FAQ>();

        public FAQServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // ==============================
        // GET ALL FAQs
        // ==============================
        public async Task<MainResponse<FAQ>> getFAQsList()
        {
            MainResponse<FAQ> response = new MainResponse<FAQ>();

            var faqs = await _unitOfWork.FAQs.GetAll();
            response.acceptedObjects = faqs.ToList();

            return response;
        }

        // ==============================
        // GET FAQ BY ID
        // ==============================
        public async Task<MainResponse<FAQ>> getFAQ(int id)
        {
            MainResponse<FAQ> response = new MainResponse<FAQ>();

            var faq = await _unitOfWork.FAQs.GetById(id);

            if (faq == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<FAQ> { faq };
            return response;
        }

        // ==============================
        // ADD FAQ
        // ==============================
        public async Task<MainResponse<FAQ>> addFAQ(FAQDTO faqDTOs)
        {
            MainResponse<FAQ> response = new MainResponse<FAQ>();

            try
            {
                var validList = await ValidateDTO.FAQDTO(faqDTOs);

                List<FAQ> faqList = new List<FAQ>();
                List<FAQ> rejectedFaqs =
                    _mapper.Map<List<FAQ>>(validList.rejectedObjects);

                foreach (var dto in validList.acceptedObjects)
                {
                    var faq = _mapper.Map<FAQ>(dto);

                    //  FILE SAVE LOGIC 
                    if (dto.Attachment != null && dto.Attachment.Length > 0)
                    {
                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "upload_FAQ");

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var fileName =  (dto.Attachment.FileName);

                        var filePath = Path.Combine(folderPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await dto.Attachment.CopyToAsync(stream);
                        }

                        // Save relative path (best practice)
                        faq.Attachment = "/upload_FAQ/" + fileName;
                    }
                        //===================================================

                    faqList.Add(faq);
                }

                if (faqList.Count > 0)
                {
                    await _unitOfWork.FAQs.Add(faqList);
                    response.acceptedObjects = faqList;
                }

                if (rejectedFaqs != null && rejectedFaqs.Count > 0)
                {
                    response.rejectedObjects = rejectedFaqs;
                    response.errors = validList.errors;
                }

                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);

                if (ex.InnerException != null)
                    response.errors.Add(ex.InnerException.Message);

                return response;
            }
        }

        //addFAQ BY Excel
        public async Task<MainResponse<FAQ>> ImportFromExcel(IFormFile excelFile , string createdby )
        {
            var response = new MainResponse<FAQ>();

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                {
                    response.errors.Add("Excel file is empty.");
                    return response;
                }

                var docsToAdd = new List<FAQ>();

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
                    var error = worksheet.Cells[r, 1].Text?.Trim();
                    var details = worksheet.Cells[r, 2].Text?.Trim();
                    

                    if (string.IsNullOrWhiteSpace(error))
                        continue;

                    //  DTO creation
                    var dto = new FAQDTO
                    {
                        Error = error,
                        Details = details
                    };

                    //  AutoMapper mapping
                    var document = _mapper.Map<FAQ>(dto);

                    // extra safety
                    //document.Attachment = null;
                    document.CreatedBy = createdby;
                    docsToAdd.Add(document);
                }

                if (!docsToAdd.Any())
                {
                    response.errors.Add("No valid rows found.");
                    return response;
                }

                foreach (var doc in docsToAdd)
                    await _unitOfWork.FAQs.Add(doc);

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


        // ==============================
        // UPDATE FAQ
        // ==============================
        public async Task<MainResponse<FAQ>> updateFAQ(int id, FAQDTO faqUpdated)
        {
            var response = new MainResponse<FAQ>();

            try
            {
                var validList = await ValidateDTO.FAQDTO(faqUpdated, true);

                var existingFAQ =
                    await _unitOfWork.FAQs.GetFirst(f => f.Id == id);

                if (existingFAQ is null)
                {
                    response.errors.Add($"Cannot find FAQ with Id {id}.");

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(
                            _mapper.Map<List<FAQ>>(validList.rejectedObjects));

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    return response;
                }

                if (validList.acceptedObjects == null ||
                    validList.acceptedObjects.Count == 0)
                {
                    response.errors.Add("No valid payload to update FAQ.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(
                            _mapper.Map<List<FAQ>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                // ================= FILE HANDLING =================
                if (dto.Attachment != null && dto.Attachment.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "upload_FAQ");

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // 🔴 Delete old file
                    if (!string.IsNullOrEmpty(existingFAQ.Attachment))
                    {
                        var oldPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            existingFAQ.Attachment.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                        );

                        if (File.Exists(oldPath))
                            File.Delete(oldPath);
                    }

                    // 🟢 UNIQUE file name (VERY IMPORTANT)
                    var fileName =  dto.Attachment.FileName;

                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.Attachment.CopyToAsync(stream);
                    }

                    existingFAQ.Attachment = "/upload_FAQ/" + fileName;
                }
                // =================================================

                // 🧠 IMPORTANT: prevent overwriting Attachment with null
                var oldFilePath = existingFAQ.Attachment;

                _mapper.Map(dto, existingFAQ);

                if (dto.Attachment == null)
                {
                    existingFAQ.Attachment = oldFilePath;
                }

                await _unitOfWork.FAQs.Update(existingFAQ);

                response.acceptedObjects.Add(existingFAQ);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(
                        _mapper.Map<List<FAQ>>(validList.rejectedObjects));

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
        }        // ==============================
        // DELETE FAQ
        // ==============================
        public async Task<MainResponse<FAQ>> deleteFAQ(int id)
        {
            MainResponse<FAQ> response = new MainResponse<FAQ>();

            var faq =
                await _unitOfWork.FAQs.DeletePhysical(f => f.Id == id);

            if (faq == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<FAQ> { faq.First() };
            return response;
        }
        public async Task<MainResponse<FAQ>> deleteGroup(List<int> ids)
        {
            MainResponse<FAQ> response = new MainResponse<FAQ>();

            try
            {
                foreach (var id in ids)
                {
                    var deletedDocuments = await _unitOfWork.FAQs.DeletePhysical(p => p.Id == id);
                    if (deletedDocuments == null || !deletedDocuments.Any())
                    {
                        response.errors?.Add($"id = {id} not found");
                        return response;
                    }
                    else
                    {
                        response.acceptedObjects = deletedDocuments.ToList();
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