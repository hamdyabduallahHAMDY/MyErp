namespace MyErp.Core.Models;

public class Treasury : Common
{
    public int Code { get; set; }             // e.g., "1000" for Asset, "1100" for Cash
    public string Name { get; set; }             // e.g., "Cash", "Inventory - Cairo"
    public int? ParentId { get; set; }           // null if top-level (Asset, Liability, etc.)
    public Treasury Parent { get; set; }
    public AccountType Type { get; set; }        // Enum: Asset, Liability, Revenue, etc.
    public bool IsActive { get; set; } = true;
}

