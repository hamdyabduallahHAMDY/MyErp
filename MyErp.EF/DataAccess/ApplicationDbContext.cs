using Logger;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyErp.Core.Models;
using Org.BouncyCastle.Utilities;

namespace MyErp.EF.DataAccess;

public class ApplicationDbContext : IdentityDbContext
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
 
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<ToDo> ToDos { get; set; }
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

