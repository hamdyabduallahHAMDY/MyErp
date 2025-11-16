namespace MyErp.Core.DTO;

public class CashAndBanksDTO
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string location { get; set; }
    public string Note1 { get; set; }
    public string Note2 { get; set; }
    public decimal AccountNo { get; set; }
    public int CurrencyId { get; set; }
    public string currencyname { get; set; }
    public string exchangeRate { get; set; }
    public string madeen { get; set; }
    public decimal treasuryCode { get; set; }
    public Boolean isActive { get; set; } = true;
    public decimal CurrentBalance { get; set; } = 0;


}
