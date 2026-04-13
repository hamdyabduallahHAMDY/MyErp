using Microsoft.AspNetCore.Identity;
using MyErp.Core.Models;
using System.Security.Claims;
using System.Text.Json;
using Type = MyErp.Core.Models.Type;

namespace MyErp.Api
{
    public class RightsModel
    {
        public List<string>? allowance { get; set; }

    }
    public class RightsModelServices
    {
        public (string currentUser, List<string> allowedUsers, bool isAuthenticated, Type userType)
            GetAccessData(ClaimsPrincipal user)
        {
            var currentUser = user.Identity?.Name;

            if (string.IsNullOrEmpty(currentUser))
            {
                return (null, new List<string>(), false, Type.Sales);
            }

            // ✅ FIXED: use the passed user
            var userType = user.GetUserType();

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
            if (!allowedUsers.Contains(currentUser))
                allowedUsers.Add(currentUser);

            return (currentUser, allowedUsers, true, userType);
        }
    }

     public static class ClaimsExtensions
        {
            public static Type GetUserType(this ClaimsPrincipal user)
            {
                var value = user.Claims
                    .FirstOrDefault(c => c.Type == "userType")?.Value;

                if (!Enum.TryParse<Type>(value, true, out var result))
                {
                    throw new Exception("Invalid user type in token");
                }

                return result;
            }
        }


     public class GetUSerId
    {
        private readonly UserManager<User> _userManager;

        public GetUSerId(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string?> GetUserIdByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return null;

            return user.Id;
        }


    }


}
