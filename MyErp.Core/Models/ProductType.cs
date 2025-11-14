namespace MyErp.Core.Models;

public class ProductType : Common
{
    public int internalId { get; set; }
    public Category Category { get; set; }
    public int CategoryId { get; set; }
    public string name { get; set; }
    public string Notes { get; set; }
}

