namespace MyErp.Core.DTO;

public class UserDTO
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string? Rights { get; set; }
    public int? GroupRolId { get; set; }
}
