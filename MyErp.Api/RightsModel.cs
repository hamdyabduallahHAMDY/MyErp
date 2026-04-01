using System.Security.Claims;
using System.Text.Json;

namespace MyErp.Api
{
    public class RightsModel
    {
        public List<string>? allowance { get; set; }
    }
    public class RightsModelServices
    {
        public (string currentUser, List<string> allowedUsers, bool isAuthenticated) GetAccessData(ClaimsPrincipal user)
        {
            var currentUser = user.Identity?.Name;
            
            if (string.IsNullOrEmpty(currentUser))
            {
                return (null, new List<string>(), false);
            }

            var rightsJson = user.Claims
                .FirstOrDefault(c => c.Type == "allowance")?.Value;

            List<string> allowedUsers = new List<string>();

            if (!string.IsNullOrEmpty(rightsJson))
            {
                var rights = JsonSerializer.Deserialize<RightsModel>(rightsJson);
                 
                if (rights?.allowance != null)
                    allowedUsers = rights.allowance;
            }

            // Always include current user
            allowedUsers.Add(currentUser);

            return (currentUser, allowedUsers, true);
        }
    }
}
