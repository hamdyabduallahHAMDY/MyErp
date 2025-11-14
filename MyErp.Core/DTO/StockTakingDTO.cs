namespace MyErp.Core.DTO;

public class StockTakingDTO
{
    public string code { get; set; }
    public int internalId { get; set; }
    public int EmployeeId { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int userId { get; set; }
    public int CategoryId { get; set; }
    public int stockId { get; set; }
    public Boolean tax = false;
    public string Curenncy { get; set; }
    public int exchangeRate { get; set; }
    public decimal Total { get; set; }
}
