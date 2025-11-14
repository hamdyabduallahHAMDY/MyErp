namespace MyErp.Core.Models;

public class StockActionDetails : Common
{
    public virtual StockActions StockActions { get; set; }
    public int? StockActionsId { get; set; }
    public virtual StockActiontransfer StockActiontransfer { get; set; }
    public int? StockActiontransferId { get; set; }
    public Orderme Orderme { get; set; }
    public int? OrdermeId { get; set; }
    public string internalId { get; set; }
    public int FinalValue { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
    public int? StockId { get; set; }
    public string? productName { get; set; }
    public string? UnitTypes { get; set; }
    public decimal price { get; set; }
    public int qty { get; set; }
    public string Notes { get; set; }
    public decimal Total { get; set; }
    public DateTime? createdProddate { get; set; }
    public DateTime? expiredate { get; set; }
    public string? serialnumber { get; set; }
}

