namespace MyErp.Core.Models;

public class Employee : Common
{
    public int InternalId { get; set; }
    public string Name { get; set; }
    public Title title { get; set; }
    public string phoneNum { get; set; }
    public string city { get; set; }
}

