namespace MyErp.Core.Models;

public class Ordermedetail : Common
{
    public string Code { get; set; }
    public Orderme Orderme { get; set; }
    public int OrdermeId { get; set; }
    public string internalId { get; set; }
    public Product Product { get; set; }
    public string? Productname { get; set; }
    public int ProductId { get; set; }
    public decimal beforeTax { get; set; }
    public decimal afterTax { get; set; }
    public int qty { get; set; }
    public decimal price { get; set; }
    public decimal discount { get; set; }
    public decimal tax { get; set; }
    public decimal total { get; set; }
    public string unitcode { get; set; }
    public Category category { get; set; }
    public int categoryId { get; set; }
    public DateTime? createdProddate { get; set; }
    public DateTime? expiredate { get; set; }
    public string? serialnumber { get; set; }

}

