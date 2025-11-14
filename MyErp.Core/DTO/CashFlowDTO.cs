namespace MyErp.Core.DTO;

public class CashFlowDTO
{
    public int CashAndBankId { get; set; }
    public int OrdermeId { get; set; }
    public string ordertype { get; set; }
    public decimal Amount { get; set; }
    public bool IsInflow { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Customer { get; set; }
}
