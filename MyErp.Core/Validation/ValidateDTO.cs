using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
namespace MyErp.Core.Validation
{
    public static class ValidateDTO
    {
    
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
        public async static Task<MainResponse<TicketDTO>> TicketDTO(List<TicketDTO> insertDTO, bool isupdate = false)
        {
            MainResponse<TicketDTO> response = new MainResponse<TicketDTO>();
            Errors<TicketDTO> err = new Errors<TicketDTO>();
            bool hasError = false;
            foreach (var user in insertDTO)
            {
                var DBuser = await ADO.GetExecuteQueryMySql<Models.Ticket>($"select * from Tickets  where Description = '{user.Description}'");
                if (DBuser.Count() > 0 && !isupdate)
                {
                    response.errors.Add(err.ObjectErrorInvExist(user.TaxRegistrationId.ToString()));
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
    }

}
