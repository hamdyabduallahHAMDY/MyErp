using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.EF.DataAccess;
using Mysqlx.Crud;
using Org.BouncyCastle.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace MyErp.EF.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
     
        public ICmd<User> Users { get; private set; }
        public ICmd<Ticket> Tickets { get; private set; }   
        public ICmd<Contract> Contracts { get; private set; }
        public ICmd<UserSession> UserSessions { get; private set; } 
        public ICmd<Customer> Customers { get; private set; } 
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
           
            Users = new Cmd<User>(_context);
           
            Tickets = new Cmd<Ticket>(_context);

            Contracts = new Cmd<Contract>(_context);

            UserSessions = new Cmd<UserSession>(_context);

            Customers = new Cmd<Customer>(_context);

        }
        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }
        /*  public int Complete()
          {
              return _context.SaveChanges();
          }  
         */

        public void Dispose()
        {
            _context.Dispose();
        }
    

}
}
