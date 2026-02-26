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

        public async Task<MainResponse<Ticket>> getTicketByStatus(int status)
        {
            MainResponse<Ticket> response = new MainResponse<Ticket>();
            var tickets = await _unitOfWork.Tickets.GetBy(x => ((int)x.Status) == status);

            if (tickets == null)
            {
                string error = Errors.ObjectNotFound();
                response.errors = new List<string> { error };
                return response;
            }

            response.acceptedObjects = tickets.ToList();
            return response;
        }
        public async Task<MainResponse<Ticket>> updateTicket(int id, TicketDTO userUpdated, string apiRootPath)
        {
            var response = new MainResponse<Ticket>();

            try
            {
                var validList = await ValidateDTO.TicketDTO(userUpdated, true);

                var existingTicket = await _unitOfWork.Tickets.GetFirst(a => a.Id == id);
                if (existingTicket is null)
                {
                    response.errors.Add($"Cannot find Ticket with ID {id}.");
                    if (validList.rejectedObjects?.Any() == true && validList.errors?.Any() == true)
                        response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<Ticket>>(validList.rejectedObjects));

                    return response;
                }

                if (validList.acceptedObjects is null || validList.acceptedObjects.Count == 0)
                {
                    response.errors.Add("No valid payload to update Ticket. Fix validation errors and try again.");
                    if (validList.errors?.Any() == true) response.errors.AddRange(validList.errors);

                    if (validList.rejectedObjects?.Any() == true)
                        response.rejectedObjects.AddRange(_mapper.Map<List<Ticket>>(validList.rejectedObjects));

                    return response;
                }

                var dto = validList.acceptedObjects[0];

                _mapper.Map(dto, existingTicket);

                if (dto.Attachment != null && dto.Attachment.Length > 0)
                {
                    string uploadsFolder = Path.Combine(apiRootPath, "Upload_Ticket");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string originalName = Path.GetFileName(dto.Attachment.FileName);
                    string fullPath = Path.Combine(uploadsFolder, originalName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await dto.Attachment.CopyToAsync(stream);
                    }

                    existingTicket.Attachment = dto.Attachment.FileName;

                }


                await _unitOfWork.Tickets.Update(existingTicket);

                response.acceptedObjects ??= new List<Ticket>();
                response.acceptedObjects.Add(existingTicket);

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

        public async Task<MainResponse<Ticket>> addTicket(TicketDTO dto, string apiRootPath)
        {
            MainResponse<Ticket> response = new MainResponse<Ticket>();
            try
            {


                response.acceptedObjects = new List<Ticket>();

                string savedFilePath = null;

                if (dto.Attachment != null && dto.Attachment.Length > 0)
                {
                    // Create Uploads folder inside API root
                    string uploadsFolder = Path.Combine(apiRootPath, "Upload_Ticket");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Create unique filename

                    string fullPath = Path.Combine(uploadsFolder, dto.Attachment.FileName    /*uniqueFileName*/);

                    // Save file physically
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await dto.Attachment.CopyToAsync(stream);
                    }

                    // Save relative path in DB
                    savedFilePath = Path.Combine("Upload_Ticket" /*uniqueFileName*/);
                }

                Ticket ticket = new Ticket
                {
                    TaxRegistrationId = dto.TaxRegistrationId,
                    Attachment = dto.Attachment.FileName,
                    TaxRegistrationName = dto.TaxRegistrationName,
                    Description = dto.Description,
                    Status = (Status)dto.Status
                }; 

                await _unitOfWork.Tickets.Add(ticket);
                //await _unitOfWork.sav();

                response.acceptedObjects.Add(ticket);

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

