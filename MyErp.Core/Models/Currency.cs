namespace MyErp.Core.Models;

public class Currency : Common
{
    public string InternalId { get; set; }
    public string name { get; set; }
    public string notes { get; set; }
    public string ExchangeRate { get; set; }
}

