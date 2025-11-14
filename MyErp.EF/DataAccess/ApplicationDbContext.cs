using Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyErp.Core.Models;
using Org.BouncyCastle.Utilities;

namespace MyErp.EF.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        try
        {
            //Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().Contains("a network-related or instance-specific error occurred while establishing a connection to sql server"))
            {
                Logs.Log("???? ????? ?? ??????? ?????? ????????");
            }
            else
                Logs.Log("\t\t???? ????? ?? ??????? ?????? ????????\n\n" + ex.Message);
        }
    }
    #region
    public DbSet<Area> Areas { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<CashAndBanks> CashAndBanks { get; set; }
    public DbSet<CashFlow> CashFlows { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    //public DbSet<ENUMS> ENUMS { get; set; }
    public DbSet<Orderme> Ordermes { get; set; }
    public DbSet<Ordermedetail> Ordermedetails { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<SalesMan> SalesMen { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<StockActionDetails> StockActionDetails { get; set; }
    public DbSet<StockActions> StockActions { get; set; }
    public DbSet<StockActiontransfer> StockActiontransfers { get; set; }
    public DbSet<StockReq> StockReqs { get; set; }
    public DbSet<StockReqDetail> StockReqDetails { get; set; }
    public DbSet<StockTaking> StockTakings { get; set; }
    public DbSet<Treasury> Treasuries { get; set; }
    public DbSet<User> Users { get; set; }
    #endregion
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    public void InitializeDatabase()
    {
        // Check if the database exists
        if (!Database.CanConnect())
        {
            try
            {
                // Create the database and all tables
                Database.EnsureCreated();

            }
            catch (Exception ex)
            {

                //  Logs.Log($"Error creating database: {ex.Message}");
            }
        }
    }


}

