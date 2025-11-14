namespace MyErp.Core.DTO;

public class StockActiontransferDTO
{
    public string physicalinvNumber { get; set; }
    public int EmployeeId { get; set; }
    public string EmpName { get; set; }
    public int CurrencyId { get; set; }
    public string CurName { get; set; }
    public int UserId { get; set; }
    public string UseName { get; set; }
    public int StockId { get; set; }
    public int ToStockId { get; set; }
    public string StoName { get; set; }
    public string StoName2 { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public decimal Total { get; set; }
}
