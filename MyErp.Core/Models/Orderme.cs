namespace MyErp.Core.Models;

public class Orderme : Common
{
    public string? internalId { get; set; }
    public OrderType? orderType { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public string? Notes2 { get; set; }
    public Currency Currency { get; set; }
    public int CurrencyId { get; set; }
    public int? currencyBalance { get; set; }
    public string? treasuryAcc { get; set; }
    public int customerId { get; set; }
    public decimal? CreditPayment { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string? Notes1 { get; set; }
    public string? CustomerName { get; set; }
    public Stock Stock { get; set; }
    public int StockId { get; set; }
    public string? EmployeeId { get; set; }
    public paymentmesod? wayofpayemnt { get; set; }
    public decimal totDiscount { get; set; }
    public decimal TotalPrice { get; set; }
    public List<Ordermedetail> Ordermedetails { get; set; }
    // public List<JornalEntry> JornalEntry { get; set; } = new List<JornalEntry>();
    public CashAndBanks CashAndBank { get; set; }
    public int CashAndBankId { get; set; }
    public string? barcode { get; set; }
    public int? POSId { get; set; } = null;
    public string? receipt { get; set; }
    public string? taxcode { get; set; }
    public string? invoice { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerCountry { get; set; }
    public string? AccountRec { get; set; }
    public string? SalesAcc { get; set; }
    public BaseOrderType? baseordertype { get; set; } = null;
    public int? offerprice { get; set; } = null;
}

