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
        public async Task<MainResponse<FAQ>> addFAQ(List<FAQDTO> faqDTOs)
        {
            MainResponse<FAQ> response = new MainResponse<FAQ>();

            try
            {
                var validList = await ValidateDTO.FAQDTO(faqDTOs);

                List<FAQ> faqList =
                    _mapper.Map<List<FAQ>>(validList.acceptedObjects);

                List<FAQ> rejectedFaqs =
                    _mapper.Map<List<FAQ>>(validList.rejectedObjects);

                if (faqList != null && faqList.Count > 0)
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

        // ==============================
        // UPDATE FAQ
        // ==============================
        public async Task<MainResponse<FAQ>> updateFAQ(int id, List<FAQDTO> faqUpdated)
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
                    response.errors.Add("No valid payload to update FAQ. Fix validation errors and try again.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(
                            _mapper.Map<List<FAQ>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingFAQ);

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
        }

        // ==============================
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
    }
}