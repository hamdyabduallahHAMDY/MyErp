namespace MyErp.Core.Models;

public class StockActiontransfer : Common
{
    public List<StockActionDetails> StockActionDetails { get; set; }
    public string physicalinvNumber { get; set; }
    public Employee Employee { get; set; }
    public int EmployeeId { get; set; }
    public string EmpName { get; set; }
    public Currency Currency { get; set; }
    public int CurrencyId { get; set; }
    public string CurName { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public string UseName { get; set; }
    public Stock Stock { get; set; }
    public int StockId { get; set; }
    public Stock ToStock { get; set; }
    public int ToStockId { get; set; }
    public string StoName { get; set; }
    public string StoName2 { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
}

