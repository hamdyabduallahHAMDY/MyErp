namespace MyErp.Core.Models;

public class CashFlow : Common
{
    // ?? Linked Cash or Bank account
    public int CashAndBankId { get; set; }
    public CashAndBanks CashAndBank { get; set; }

    // ?? Linked Order (source transaction)
    public int OrdermeId { get; set; }
    public Orderme Orderme { get; set; }
    public string ordertype { get; set; }
    // ?? Transaction amount
    public decimal Amount { get; set; }

    // ?? Direction of money movement (true = inflow, false = outflow)
    public bool IsInflow { get; set; }

    // ?? Balance snapshot after this transaction
    public decimal BalanceAfter { get; set; }

    // ?? Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Customer { get; set; }
}

