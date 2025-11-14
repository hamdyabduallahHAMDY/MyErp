using MyErp.Core.Models;

namespace MyErp.Core.DTO;

public class TreasuryDTO
{
    public int Code { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public AccountType Type { get; set; }
    public bool IsActive { get; set; } = true;
}
