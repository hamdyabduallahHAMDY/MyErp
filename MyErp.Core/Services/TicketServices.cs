using AutoMapper;
using Google.Protobuf.WellKnownTypes;
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
    public class TicketServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        Errors<User> Errors = new Errors<User>();
        public TicketServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<MainResponse<Ticket>> getTicketsList()
        {
            MainResponse<Ticket> response = new MainResponse<Ticket>();
            var user = await _unitOfWork.Tickets.GetAll();
            if (!user.Any())
            {
                response.errors.Add("There is no Tickets");
            }
            response.acceptedObjects = user.ToList();
            return response;
        }
        public async Task<MainResponse<Ticket>> getTicket(int id)
        {
            MainResponse<Ticket> response = new MainResponse<Ticket>();
            var user = await _unitOfWork.Tickets.GetById(id);

            if (user == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = new List<Ticket> { user };
            return response;
        }
        public async Task<MainResponse<Ticket>> updateTicket(int id, List<TicketDTO> userUpdated)
        {
            var response = new MainResponse<Ticket>();

            try
            {
                var validList = await ValidateDTO.TicketDTO(userUpdated, true);

                var existingUser = await _unitOfWork.Tickets.GetFirst(a => a.Id == id);
                if (existingUser is null)
                {
                    response.errors.Add($"Cannot find User with ID {id}.");
                    if (validList.rejectedObjects?.Any() == true)
                        if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                    response.rejectedObjects.AddRange(_mapper.Map<List<Ticket>>(validList.rejectedObjects));
                    return response;
                }

                if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
                {
                    response.errors.Add("No valid payload to update User. Fix validation errors and try again.");
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);
                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<Ticket>>(validList.rejectedObjects));
                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingUser);

                await _unitOfWork.Tickets.Update(existingUser);

                response.acceptedObjects.Add(existingUser);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(_mapper.Map<List<Ticket>>(validList.rejectedObjects));
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

        public async Task<MainResponse<Ticket>> addTicket(TicketDTO dto)
        {
            MainResponse<Ticket> response = new MainResponse<Ticket>();

            try
            {
                response.acceptedObjects = new List<Ticket>();


                byte[] filebytes = null;

                if (dto.attachment != null && dto.attachment.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await dto.attachment.CopyToAsync(ms);
                        filebytes = ms.ToArray();
                    }
                }

                Ticket ticket = new Ticket
                {
                    Description = dto.Description,
                    TaxRegistrationId = dto.TaxRegistrationId,
                    Attachment = filebytes,
                    TaxRegistrationName = dto.TaxRegistrationName
                };

                await _unitOfWork.Tickets.Add(ticket);

                response.acceptedObjects.Add(ticket);


                return response;
            }
            catch (Exception ex)
            {
                Logs.Log(ex.ToString());
                response.errors = new List<string> { ex.Message };
                return response;
            }
        }

        public async Task<MainResponse<Ticket>> deleteUser(int id)
        {
            MainResponse<Ticket> response = new MainResponse<Ticket>();

            var user = await _unitOfWork.Tickets.DeletePhysical(p => p.Id == id);

            if (user == null)
            {
                string error = Errors.ObjectNotFoundWithId(id);
                response.errors = new List<string> { error };
                return response;
            }
            response.acceptedObjects = new List<Ticket> { user.First() };
            return response;
        }
    }
}

