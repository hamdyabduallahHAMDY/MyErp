using Microsoft.AspNetCore.Identity;

namespace MyErp.Core.Models;

public class User : IdentityUser
{
    //public string Name { get; set; }
    //public string password { get; set; }
    public string? Rights { get; set; }
    public string? allowance { get; set; }
    public string? registrationTaxid { get; set; }
    
}

