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
        ICmd<StockReq> StockReqs { get; }
        ICmd<StockReqDetail> StockReqDetails { get; }
        ICmd<CashFlow> CashFlows { get; }
        ICmd<CashAndBanks> CashAndBankss { get; }
        ICmd<Treasury> Treasurys { get; }
        ICmd<Orderme> Ordermes { get; }
        ICmd<Ordermedetail> Ordermedetails { get; }
        ICmd<StockActiontransfer> StockActiontransfers { get; }
        ICmd<StockActionDetails> StockActionDetailss { get; }
        ICmd<StockActions> StockActionss { get; }
        ICmd<Employee> Employees { get; }
        ICmd<StockTaking> StockTakings { get; }
        ICmd<ProductType> ProductTypes { get; }
        ICmd<Currency> Currencys { get; }
        ICmd<Area> Areas { get; }
        ICmd<SalesMan> SalesMen { get; }
        ICmd<Product> Products { get; }
        ICmd<Customer> Customers { get; }
        ICmd<Branch> Branchs { get; }
        ICmd<Stock> Stocks { get; }    
        ICmd<User> Users { get; }
        ICmd<Category> Categories { get; }
        Task<int> Complete();

    }
}
