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
        public ICmd<StockReq> StockReqs { get; set; }
        public ICmd<StockReqDetail> StockReqDetails { get; set; }   
        public ICmd<CashFlow> CashFlows { get; set; }
        public ICmd<Treasury> Treasurys { get; set; }
        public ICmd<CashAndBanks> CashAndBankss { get; set; }
        public ICmd<Orderme> Ordermes { get; set; }
        public ICmd<Ordermedetail> Ordermedetails { get; set; }
        public ICmd<StockActiontransfer> StockActiontransfers { get; set; }
        public ICmd<StockActionDetails> StockActionDetailss { get; set; }
        public ICmd<StockActions> StockActionss { get; set; }
        public ICmd<ProductType> ProductTypes { get; set; }
        public ICmd<Currency> Currencys { get; set; }
        public ICmd<Employee> Employees { get; set; }
        public ICmd<StockTaking> StockTakings { get; set; }
        public ICmd<Area> Areas { get; set; }
        public ICmd<SalesMan> SalesMen { get; set; }
        public ICmd<Product> Products { get; private set; }
        public ICmd<Customer> Customers { get; private set; }        
        public ICmd<Branch> Branchs { get; private set; }
        public ICmd<Stock> Stocks { get; private set; }
        public ICmd<User> Users { get; private set; }
        public ICmd<SalesMan> SalesMan { get; private set; }   
        public ICmd<Category> Categories { get; set; }
        

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            StockReqs = new Cmd<StockReq>(_context);
            StockReqDetails = new Cmd<StockReqDetail>(_context);
          
            CashFlows = new Cmd<CashFlow>(_context);
            
            CashAndBankss = new Cmd<CashAndBanks>(_context);
            Treasurys = new Cmd<Treasury>(_context);
            Ordermes = new Cmd<Orderme>(_context);
            Ordermedetails = new Cmd<Ordermedetail>(_context);
            StockActiontransfers = new Cmd<StockActiontransfer>(_context);
            StockActionDetailss = new Cmd<StockActionDetails>(_context);
            StockActionss = new Cmd<StockActions>(_context);
            StockTakings = new Cmd<StockTaking>(_context);
            Employees = new Cmd<Employee>(_context);
            Currencys = new Cmd<Currency>(_context);
            ProductTypes = new Cmd<ProductType>(_context);
          
            Areas = new Cmd<Area>(_context);
            SalesMen = new Cmd<SalesMan>(_context);
            Products = new Cmd<Product>(_context);
            Customers = new Cmd<Customer>(_context);
         
            Branchs = new Cmd<Branch>(_context);
            Stocks = new Cmd<Stock>(_context);
         
            Users = new Cmd<User>(_context);
           
            Categories = new Cmd<Category>(_context);
          
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
