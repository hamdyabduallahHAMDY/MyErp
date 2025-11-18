using MyErp.Core.Models;

namespace MyErp.Core.DTO;

public class OrdermeDTO
{
    public string? internalId { get; set; }
    public OrderType orderType { get; set; }
    public int UserId { get; set; }
    public string? Notes2 { get; set; }
    public int CurrencyId { get; set; }
    public int? currencyBalance { get; set; }
    public string? treasuryAcc { get; set; }
    public int customerId { get; set; }
    public decimal? CreditPayment { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
    public string? Notes1 { get; set; }
    public string? CustomerName { get; set; }
    public int StockId { get; set; }
    public string? EmployeeId { get; set; }
    public paymentmesod? wayofpayemnt { get; set; }
    public decimal totDiscount { get; set; }
    public decimal TotalPrice { get; set; }
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
public class OrderCreateDTO
{
    public int? baseordertype { get; set; } = null;
    public string? InternalId { get; set; }
    public OrderType? OrderType { get; set; }
    public decimal? CreditPayment { get; set; }
    public int UserId { get; set; }
    public int customerId { get; set; }
    public int CurrencyId { get; set; }
    public int StockId { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime? OrderDate { get; set; }
    public decimal totDiscount { get; set; }
    public paymentmesod? wayofpayemnt { get; set; } = null;
    public string? Notes1 { get; set; }
    public List<OrdermedetailDTO> OrdermeDetails { get; set; }
    public int CashAndBankId { get; set; }
    public string? barcode { get; set; }
    public int? POSId { get; set; } = null;
    public string? receipt { get; set; }
    public string? taxcode { get; set; }
    public string? invoice { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerCountry { get; set; }
    public string? CustomerName { get; set; }
    public string? Notes2 { get; set; }
    public string? AccountRec { get; set; }
    public string? SalesAcc { get; set; }
    public decimal TotalPrice { get; set; }
    public int? offerprice { get; set; } = null;

}
public class ProductSalesDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int QtySold { get; set; }
}
public class StagnantProductDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public DateTime? LastSoldAt { get; set; }           // null => never sold
    public int DaysSinceLastSale { get; set; }          // if never sold: large number (e.g., int.MaxValue/365)
    public int TotalQtySoldEver { get; set; }           // for quick reference
}
public class POSOrderRequest
{
    public int CustomerId { get; set; }
    public int CurrencyId { get; set; }
    public int CashandBankId { get; set; }
    public List<POSProductItem> Products { get; set; } = new();
}
public class ProductMovementDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string MovementType { get; set; }  // e.g. "StockIn", "StockOut", "TransferOut", "TransferIn"
    public int? FromStockId { get; set; }
    public string FromStockName { get; set; }
    public int? ToStockId { get; set; }
    public string ToStockName { get; set; }
    public double Quantity { get; set; }
    public DateTime Date { get; set; }
}
public class POSProductItem
{
    public int ProductId { get; set; }
    public int Qty { get; set; }
}