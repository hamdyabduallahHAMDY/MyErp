namespace MyErp.Core.Models;

public class User : Common
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string? Rights { get; set; }
    public int? GroupRolId { get; set; }
 //   public virtual GroupRol GroupRol { get; set; }
}

