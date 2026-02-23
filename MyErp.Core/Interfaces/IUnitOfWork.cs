using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MyErp.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyErp.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
       
        ICmd<User> Users { get; }
        ICmd<Ticket> Tickets { get; }
        ICmd<Contract> Contracts { get; }
        ICmd<UserSession> UserSessions { get; }
        ICmd<Customer> Customers { get; }
        Task<int> Complete();

    }
}
