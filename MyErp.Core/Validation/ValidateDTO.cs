using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
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
                var DBcustomer = await ADO.GetExecuteQueryMySql<Models.Customer>($"select * from Customers where Name = '{customer.Name}'");
                if (DBcustomer.Count() > 0 && !isupdate)
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
        public async static Task<MainResponse<ToDoDTO>> ToDoDTO(List<ToDoDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<ToDoDTO> response = new MainResponse<ToDoDTO>();
            Errors<ToDoDTO> err = new Errors<ToDoDTO>();
            bool hasError = false;
            foreach (var customer in insertDTO)
            {
                var DBcustomer = await ADO.GetExecuteQueryMySql<Models.ToDo>($"select * from Customers where Name = '{customer.Title}'");
                if (DBcustomer.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Title));
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
        public async static Task<MainResponse<FAQDTO>> FAQDTO(List<FAQDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<FAQDTO> response = new MainResponse<FAQDTO>();
            Errors<FAQDTO> err = new Errors<FAQDTO>();
            bool hasError = false;
            foreach (var customer in insertDTO)
            {
                var DBcustomer = await ADO.GetExecuteQueryMySql<Models.FAQ>($"select * from Customers where Name = '{customer.Error}'");
                if (DBcustomer.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(customer.Error));
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
                if (hasError)
                {
                    return response;
                }
                response.acceptedObjects.Add(doc);
            }
            return response;
        }


        public async static Task<MainResponse<UserDTO>> UserDTO(List<UserDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<UserDTO> response = new MainResponse<UserDTO>();
            Errors<UserDTO> err = new Errors<UserDTO>();
            bool hasError = false;
            foreach (var user in insertDTO)
            {
                var DBuser = await ADO.GetExecuteQueryMySql<Models.User>($"select * from Users  where Name = '{user.Name}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(user.Name));
                    response.rejectedObjects.Add(user);
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
                var DBuser = await ADO.GetExecuteQueryMySql<Models.Contract>($"select * from Contracts  where CompanyName = '{user.CompanyName}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(user.CompanyName));
                    response.rejectedObjects.Add(user);
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
        public async static Task<MainResponse<TicketDTO>> TicketDTO(TicketDTO user, bool isupdate = false)
        {
            MainResponse<TicketDTO> response = new MainResponse<TicketDTO>();
            Errors<TicketDTO> err = new Errors<TicketDTO>();
            bool hasError = false;
            { 
                var DBuser = await ADO.GetExecuteQueryMySql<Models.Ticket>($"select * from Tickets  where Description = '{user.Description}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(user.TaxRegistrationId.ToString()));
                    response.rejectedObjects.Add(user);
                    hasError = true;
                }
                if (hasError)
                {
                    response.errors.Add(err.ObjectErrorInvExist(user.Description));
                    return response;
                }
                response.acceptedObjects.Add(user);
            }
            return response;
        }
    }

}
