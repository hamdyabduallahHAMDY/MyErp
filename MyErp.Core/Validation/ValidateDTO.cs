using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
namespace MyErp.Core.Validation
{
    public static class ValidateDTO
    {
        public async static Task<MainResponse<CustomerDTO>> CustomerDTO(List<CustomerDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<CustomerDTO> response = new MainResponse<CustomerDTO>();
            Errors<CustomerDTO> err = new Errors<CustomerDTO>();
            bool hasError = false;
            foreach (var customer in insertDTO)
            {
                var DBcustomer = await ADO.GetExecuteQueryMySql<Models.Customer>($"select * from Customers where companyName = '{customer.TaxRegistrationNumber}'");
                if (DBcustomer.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Name));
                    response.rejectedObjects.Add(customer);
                    hasError = true;
                    continue;
                }
                if (!customer.AnyDesk.IsDigitsOrPlusOnly())
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Name));
                    response.rejectedObjects.Add(customer);
                    hasError = true;
                    continue;
                }
                if (!customer.Name.IsStringValidation())
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Name));
                    response.rejectedObjects.Add(customer);
                    hasError = true;
                    continue;
                }
                if (!customer.POC.IsDigitsOrPlusOnly())
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Name));
                    response.rejectedObjects.Add(customer);
                    hasError = true;
                    continue;
                }
                if (!customer.Phone.IsDigitsOrPlusOnly())
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Name));
                    response.rejectedObjects.Add(customer);
                    hasError = true;
                    continue;
                }
                if (!customer.Phone.IsDigitsOrPlusOnly())
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Name));
                    response.rejectedObjects.Add(customer);
                    hasError = true;
                    continue;
                }
                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(customer);
            }
            return response;
        }
        public async static Task<MainResponse<CalenderTaskDTO>> CalenderTaskDTO(List<CalenderTaskDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<CalenderTaskDTO> response = new MainResponse<CalenderTaskDTO>();
            Errors<CalenderTaskDTO> err = new Errors<CalenderTaskDTO>();
            bool hasError = false;
            foreach (var calen in insertDTO)
            {
                var DBcustomer = await ADO.GetExecuteQueryMySql<Models.CalenderTask>($"select * from CalenderTasks where Title = '{calen.Title}'");
                if (DBcustomer.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(calen.Title));
                    response.rejectedObjects.Add(calen);
                    hasError = true;
                    continue;
                }
                var now = DateTime.Now;

                if (calen.EndTime <= now)
                {
                    response.errors.Add("EndTime can't be before the current time.");
                    response.rejectedObjects.Add(calen);
                    hasError = true;
                    continue;
                }

                if (calen.StartTime >= calen.EndTime)
                {
                    response.errors.Add("StartTime can't be after or equal to EndTime.");
                    response.rejectedObjects.Add(calen);
                    hasError = true;
                    continue;
                }
                if (!(calen.allday == "false" || calen.allday == "true"))
                {
                    response.errors.Add("Invalid allday status.");
                    response.rejectedObjects.Add(calen);
                    hasError = true;
                    return response;

                }
                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(calen);
            }
            return response;
        }
        public async static Task<MainResponse<ToDoDTO>> ToDoDTO(List<ToDoDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<ToDoDTO> response = new MainResponse<ToDoDTO>();
            Errors<ToDoDTO> err = new Errors<ToDoDTO>();
            bool hasError = false;
            foreach (var todo in insertDTO)
            {
                if (!(todo.Title.IsStringValidation()))
                {
                    response.errors.Add(err.ObjectErrorInvExist(todo.Title));
                    response.rejectedObjects.Add(todo);
                    hasError = true;
                    continue;
                }
                if (!todo.Description.IsStringValidation())
                {
                    response.errors.Add(err.ObjectErrorInvExist(todo.Description));
                    response.rejectedObjects.Add(todo);
                    hasError = true;
                    continue;
                }
                //var assignedto = await ADO.GetExecuteQueryMySql<Models.ToDo>($"select * from AspNetUsers where Id = '{todo.AssignedTo}'");
                //if (!todo.AssignedTo.IsDigitsOrPlusOnly())
                //{
                //    response.errors.Add("Error using unexisted user ");
                //    response.rejectedObjects.Add(todo);
                //    hasError = true;
                //    continue;
                //}
                //var createdby = await ADO.GetExecuteQueryMySql<Models.ToDo>($"select * from users where Id = {todo.CreatedBy}");
                //if (!todo.CreatedBy.IsDigitsOrPlusOnly())
                //{
                //    response.errors.Add("Error using unexisted user ");
                //    response.rejectedObjects.Add(todo);
                //    hasError = true;
                //    continue;
                //}
                var DBtodo = await ADO.GetExecuteQueryMySql<Models.ToDo>($"select * from ToDos where Title = '{todo.Title}'");
                if (DBtodo.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(todo.Title));
                    response.rejectedObjects.Add(todo);
                    hasError = true;
                    continue;
                }

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(todo);
            }
            return response;
        }
        public async static Task<MainResponse<FAQDTO>> FAQDTO(List<FAQDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<FAQDTO> response = new MainResponse<FAQDTO>();
            Errors<FAQDTO> err = new Errors<FAQDTO>();
            bool hasError = false;
            foreach (var faq in insertDTO)
            {
                var DBfaq = await ADO.GetExecuteQueryMySql<Models.FAQ>($"select * from Customers where Name = '{faq.Error}'");
                if (DBfaq.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(faq.Error));
                    response.rejectedObjects.Add(faq);
                    hasError = true;
                    continue;
                }
                if (!faq.Error.IsStringValidation())
                {
                    response.errors.Add(err.ObjectErrorInvExist(faq.Error));
                    response.rejectedObjects.Add(faq);
                    hasError = true;
                    continue;
                }
                if (!faq.Details.IsStringValidation())
                {
                    response.errors.Add(err.ObjectErrorInvExist(faq.Error));
                    response.rejectedObjects.Add(faq);
                    hasError = true;
                    continue;
                }
                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(faq);
            }
            return response;
        }
        public async static Task<MainResponse<DocumentDTO>> DocumentDTO(DocumentDTO doc, bool isupdate = false)
        {
            MainResponse<DocumentDTO> response = new MainResponse<DocumentDTO>();
            Errors<DocumentDTO> err = new Errors<DocumentDTO>();
            bool hasError = false;
            {
                var DBcustomer = await ADO.GetExecuteQueryMySql<Models.Document>($"select * from Documents where Name = '{doc.Name}'");
                if (DBcustomer.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(doc.Name));
                    response.rejectedObjects.Add(doc);
                    hasError = true;

                }
                if (!doc.Name.IsStringValidation())
                {
                    response.errors.Add(err.ObjectErrorInvExist(doc.Name));
                    response.rejectedObjects.Add(doc);
                    hasError = true;
                }
                if (!doc.subject.IsStringValidation())
                {
                    response.errors.Add(err.ObjectErrorInvExist(doc.Name));
                    response.rejectedObjects.Add(doc);
                    hasError = true;
                }
                if (hasError)
                {
                    return response;
                }
                response.acceptedObjects.Add(doc);
            }
            return response;
        }
        public async static Task<MainResponse<UserSessionDTO>> UserSessionDTO(List<UserSessionDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<UserSessionDTO> response = new MainResponse<UserSessionDTO>();
            Errors<UserSessionDTO> err = new Errors<UserSessionDTO>();
            bool hasError = false;
            foreach (var user in insertDTO)
            {
                var DBuser = await ADO.GetExecuteQueryMySql<Models.UserSession>($"select * from UserSession  where CompanyName = '{user.CompanyName}'");
                //if (DBuser.Count() > 0 && !isupdate)
                //{
                //    response.errors.Add(err.ObjectErrorInvExist(user.CompanyName));
                //    response.rejectedObjects.Add(user);
                //    hasError = true;
                //    continue;
                //}

                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(user);
            }
            return response;
        }
        public async static Task<MainResponse<ContractDTO>> ContractDTO(List<ContractDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<ContractDTO> response = new MainResponse<ContractDTO>();
            Errors<ContractDTO> err = new Errors<ContractDTO>();
            bool hasError = false;
            foreach (var user in insertDTO)
            {
                var DBuser = await ADO.GetExecuteQueryMySql<Models.Contract>($"select * from Contracts  where regestrationNumber = '{user.regestrationNumber}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(user.CompanyName));
                    response.rejectedObjects.Add(user);
                    hasError = true;
                    continue;
                }
                if (user.StartDate < user.EndDate)
                {
                    response.errors.Add("StartDate Cant be after EndDate ");
                    response.rejectedObjects.Add(user);
                    hasError = true;
                    continue;
                }
                if (!user.CompanyName.IsStringValidation())
                {
                    response.errors?.Add(err.ObjectErrorInvExist(user.CompanyName));
                    response.rejectedObjects?.Add(user);
                    hasError = true;
                    continue;
                }
                if (hasError)
                {
                    continue;
                }
                response.acceptedObjects.Add(user);
            }
            return response;
        }
        public async static Task<MainResponse<TicketDTO>> TicketDTO(TicketDTO ticket, bool isupdate = false)
        {
            MainResponse<TicketDTO> response = new MainResponse<TicketDTO>();
            Errors<TicketDTO> err = new Errors<TicketDTO>();
            bool hasError = false;
            {
                if (!ticket.Description.IsStringValidation() && !isupdate)
                {
                    response.errors?.Add(err.ObjectIsStringAndNumbersOnly(ticket.Description));
                    response.rejectedObjects?.Add(ticket);
                    hasError = true;
                    return response;
                }
                var DBuser = await ADO.GetExecuteQueryMySql<Models.Ticket>($"select * from Tickets  where Description = '{ticket.Description}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(ticket.TaxRegistrationId.ToString()));
                    response.rejectedObjects.Add(ticket);
                    hasError = true;
                    return response;
                }
                if (!ticket.TaxRegistrationName.IsStringValidation())
                {
                    response.errors.Add(err.ObjectIsStringAndNumbersOnly(ticket.Description));
                    response.rejectedObjects.Add(ticket);
                    hasError = true;
                    return response;

                }
                //if (ticket.Attachment.FileName.ToString().IsTicketFileDuplicate())
                //{
                //    response.errors.Add(err.ObjectIsStringAndNumbersOnly(ticket.Description));
                //    response.rejectedObjects.Add(ticket);
                //    hasError = true;
                //    return response;

                //}
                if (!ticket.TaxRegistrationName.IsStringValidation())
                {
                    response.errors.Add(err.ObjectIsStringAndNumbersOnly(ticket.Description));
                    response.rejectedObjects.Add(ticket);
                    hasError = true;
                    return response;

                }
                if (!Enum.IsDefined(typeof(Status), ticket.Status))
                {
                    response.errors.Add("Invalid ticket status.");
                    response.rejectedObjects.Add(ticket);
                    hasError = true;
                    return response;

                }
                if (hasError)
                {
                    response.errors.Add(err.ObjectErrorInvExist(ticket.Description));
                    return response;
                }
                response.acceptedObjects.Add(ticket);
            }
            return response;
        }
        public async static Task<MainResponse<LeadDTO>> LeadDTO(LeadDTO lead, bool isupdate = false)
        {
            MainResponse<LeadDTO> response = new MainResponse<LeadDTO>();
            Errors<LeadDTO> err = new Errors<LeadDTO>();
            bool hasError = false;
            {
                var DBuser = await ADO.GetExecuteQueryMySql<Models.Lead>($"select * from Leads  where Name = '{lead.Name}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(lead.Name));
                    response.rejectedObjects.Add(lead);
                    hasError = true;
                    return response;
                }
                if (!lead.Name.IsStringValidation())
                {
                    response.errors.Add(err.ObjectIsStringAndNumbersOnly(lead.Name));
                    response.rejectedObjects.Add(lead);
                    hasError = true;
                    return response;
                }
                if (!lead.PhoneNo.IsDigitsOrPlusOnly())
                {
                    response.errors.Add(err.ObjectIsStringAndNumbersOnly(lead.Name));
                    response.rejectedObjects.Add(lead);
                    hasError = true;
                    return response;
                }
                if (!Enum.IsDefined(typeof(LeadStatus), lead.Status))
                {
                    response.errors.Add("Invalid lead status.");
                    response.rejectedObjects.Add(lead);
                    hasError = true;
                    return response;
                }
                if (hasError)
                {
                    return response;
                }
                response.acceptedObjects.Add(lead);
            }
            return response;




        }
        public async static Task<MainResponse<EmailDTO>> EmailDTO(EmailDTO email, bool isupdate = false)
        {
            MainResponse<EmailDTO> response = new MainResponse<EmailDTO>();
            Errors<EmailDTO> err = new Errors<EmailDTO>();
            bool hasError = false;
            
                var DBemail = await ADO.GetExecuteQueryMySql<Models.Email>($"select * from Emails where subject = '{email.Subject}'");
                if (DBemail.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(email.Subject));
                    response.rejectedObjects.Add(email);
                    hasError = true;
                    
                }
                if (!email.Subject.IsStringValidation())
                {
                    response.errors.Add(err.ObjectErrorInvExist(email.Subject));
                    response.rejectedObjects.Add(email);
                    hasError = true;
                }
                if (hasError)
                {

                }
                response.acceptedObjects.Add(email);
            
            return response;
        }

    }
}
