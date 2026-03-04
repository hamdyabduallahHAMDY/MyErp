using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyErp.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyErp.Api.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return BadRequest("User already exists.");

            User user = new()
            {
                Email = model.Email,
                UserName = model.Username,
                Rights = "User" // default role
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created successfully.");
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Rights", user.Rights ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        // ================= GET ALL USERS =================
        [HttpGet("get-all")]
        public IActionResult GetAllUsers()
        {
            var users = _userManager.Users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.Rights,
                u.registrationTaxid
            }).ToList();

            return Ok(users);
        }

        // ================= GET USER BY ID =================
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Rights,
                user.registrationTaxid
            });
        }


        // ================= DELETE USER =================
        [HttpDelete("delete/{name}")]
        public async Task<IActionResult> DeleteUser(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User deleted successfully.");
        }

        // ================= UPDATE RIGHTS =================
        [HttpPut("update-rights")]
        public async Task<IActionResult> UpdateRights([FromBody] UpdateRightsModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
                return NotFound("User not found.");

            user.Rights = model.Rights;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Rights updated successfully.");
        }

        // ================= CHANGE PASSWORD (USER) =================
        //[HttpPut("change-password-by-name")]
        //public async Task<IActionResult> ChangePasswordByName(
        //[FromBody] ChangePasswordByNameModel model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);

        //    if (user == null)
        //        return NotFound("User not found.");

        //    var result = await _userManager.ChangePasswordAsync(
        //        user,
        //        model.CurrentPassword,
        //        model.NewPassword
        //    );

        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);

        //    return Ok("Password changed successfully.");
        //}

        // ================= RESET PASSWORD (ADMIN) =================
        [HttpPut("reset-password-by-name")]
        public async Task<IActionResult> ResetPasswordByName(
    [FromBody] ResetPasswordByNameModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return NotFound("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                model.NewPassword
            );

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password reset successfully.");
        }
    
        [HttpPut("edit-user-full")]
        public async Task<IActionResult> EditUserFull([FromBody] UpdateUserFullModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound("User not found.");

            //bool isAdmin = User.IsInRole("Admin");

            // Update normal fields (Admin only for rights)
            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.NewUsername))
            {
                user.UserName = model.NewUsername;
                user.NormalizedUserName = model.NewUsername.ToUpper();
            }

            //if (isAdmin && model.Rights != null)
                user.Rights = model.Rights;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            //  If password change requested
            //if (!string.IsNullOrEmpty(model.NewPassword))
            //{
            //    if (isAdmin)
            //    {
                    // Admin reset
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                    if (!resetResult.Succeeded)
                        return BadRequest(resetResult.Errors);
                //}
                //else
                //{
                //    // User must provide current password
                //    if (string.IsNullOrEmpty(model.CurrentPassword))
                //        return BadRequest("Current password is required.");

                //    var changeResult = await _userManager.ChangePasswordAsync(
                //        user,
                //        model.CurrentPassword,
                //        model.NewPassword
                //    );

                //    if (!changeResult.Succeeded)
                //        return BadRequest(changeResult.Errors);
                //}
            

            return Ok("User updated successfully.");
        }
        }
    // ================= DTOs =================

    public record RegisterModel(string Username, string Email, string Password);
    public record LoginModel(string Username, string Password);
    public record UpdateRightsModel(string Username, string Rights);
    public record UpdateUserByNameModel(string Id, string? Email, string? NewUsername, string? Rights);
    public record ChangePasswordByNameModel(
        string Username,
        string CurrentPassword,
        string NewPassword
    ); 
    public record ResetPasswordByNameModel(
    string Username,
    string NewPassword
    );
    public record UpdateUserFullModel(
    string Id,
    string? Email,
    string? NewUsername,
    string? Rights,
    string? NewPassword
);
}