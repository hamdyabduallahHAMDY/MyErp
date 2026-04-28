namespace MyErp.Core.Models;

public class Common
{
    public RowStatus RowStatus { get; set; } = RowStatus.Active;
    public int Id { get; set; }
    public string? CreatedBy { get; set; }
}

