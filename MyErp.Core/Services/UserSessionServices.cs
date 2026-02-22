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
using System.Text;
using System.Threading.Tasks;

namespace MyErp.Core.Services
{
    public class UserSessionServices
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        Errors<Contract> Errors = new Errors<Contract>();




        public UserSessionServices(IUnitOfWork unitOfWork , IMapper mapper) 
        {
             _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        public async Task<MainResponse<UserSession>> GetSession()
        {
            MainResponse<UserSession> response = new MainResponse<UserSession>();

            var contracts = await _unitOfWork.UserSessions.GetAll();

            response.acceptedObjects = contracts.ToList();

            return response;
        }



        public async Task<MainResponse<UserSession>> GetSessionID(int id)
        {
            MainResponse<UserSession> response = new MainResponse<UserSession>();

            var contract = await _unitOfWork.UserSessions.GetById(id);

            if (contract == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<UserSession> { contract };

            return response;
        }



        public async Task<MainResponse<UserSession>> addSession(List<UserSessionDTO> contracts)
        {
            MainResponse<UserSession> response = new MainResponse<UserSession>();

            try
            {
                var validList = await ValidateDTO.UserSessionDTO(contracts);

                List<UserSession> contractList = _mapper.Map<List<UserSession>>(validList.acceptedObjects);
                List<UserSession> rejectedContracts = _mapper.Map<List<UserSession>>(validList.rejectedObjects);

                if (contractList != null && contractList.Count > 0)
                {
                    await _unitOfWork.UserSessions.Add(contractList);
                    response.acceptedObjects = contractList;
                }

                if (rejectedContracts != null && rejectedContracts.Count > 0)
                {
                    response.rejectedObjects = rejectedContracts;
                    response.errors = validList.errors;
                }

                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors.Add(ex.Message);
                if (ex.InnerException != null) response.errors.Add(ex.InnerException.Message);
                return response;
            }
        }



        public async Task<MainResponse<UserSession>> updateSession(int id, List<UserSessionDTO> contractUpdated)
        {
            var response = new MainResponse<UserSession>();

            try
            {
                var validList = await ValidateDTO.UserSessionDTO(contractUpdated, true);

                var existingContract = await _unitOfWork.UserSessions.GetFirst(a => a.Id == id);

                if (existingContract is null)
                {
                    response.errors.Add($"Cannot find Contract with ID {id}.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<UserSession>>(validList.rejectedObjects));

                    return response;
                }

                if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
                {
                    response.errors.Add("No valid payload to update Contract. Fix validation errors and try again.");

                    if (validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<UserSession>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingContract);

                await _unitOfWork.UserSessions.Update(existingContract);

                response.acceptedObjects.Add(existingContract);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<UserSession>>(validList.rejectedObjects));

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



        public async Task<MainResponse<UserSession>> deleteSession(int id)
        {
            MainResponse<UserSession> response = new MainResponse<UserSession>();

            var contract = await _unitOfWork.UserSessions.DeletePhysical(p => p.Id == id);

            if (contract == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<UserSession> { contract.First() };

            return response;
        }




    }
}
