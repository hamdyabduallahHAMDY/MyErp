namespace MyErp.Core.Models;

public class Customer : Common
{
    public string? InternalId { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Phone2 { get; set; }
    public string? Phone3 { get; set; }
    public string? Email { get; set; }
    public CustomerType CustomerType { get; set; }
    public EntityType EntityType { get; set; }
    public string? Country { get; set; } = "EG"; //"EG";
    public string? Governate { get; set; } = "Cairo";  //Egypt";
    public string? RegionCity { get; set; } = "City";
    public string? Street { get; set; } = "Street";
    public string? BuildingNumber { get; set; } = "11";
    public string? address { get; set; }
    public string? opening_credit { get; set; }
    public string? opening_debit { get; set; }
    public string treasurycode { get; set; }
    public string? SchemeID { get; set; }
    public string treeCode { get; set; }
    public string? parentAcc { get; set; }
    public string? AdditionalNotes { get; set; }
    public string? companyType { get; set; }
    public string? companyTaxNo { get; set; }
    public string? companyAdress { get; set; }
    public string? companyCeoName { get; set; }
    public string? companyCeoNumber { get; set; }
    public string? companyopenningbalance { get; set; }
    public string? companyName { get; set; }

}

