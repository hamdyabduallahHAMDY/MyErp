using License.Core.Helper;
using Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyErp.Core.HTTP;
using MyErp.Core.Models;
using OfficeOpenXml;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Type = MyErp.Core.Models.Type;

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
        [HttpGet("template/users")]
        public async Task<IActionResult> DownloadUsersTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users Template");

            // ================= HEADERS =================
            worksheet.Cells[1, 1].Value = "Username";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Password";
            worksheet.Cells[1, 4].Value = "Rights";
            worksheet.Cells[1, 5].Value = "TaxId";
            worksheet.Cells[1, 6].Value = "userType";

            // ================= DEMO ROW =================
            worksheet.Cells[2, 1].Value = "user1";
            worksheet.Cells[2, 2].Value = "user1@test.com";
            worksheet.Cells[2, 3].Value = "P@ssw0rd123";
            worksheet.Cells[2, 4].Value = "{\"allowance\":[\"user2\",\"user3\"]}";
            worksheet.Cells[2, 5].Value = "123456789";
            worksheet.Cells[2, 6].Value = "1";


            // ================= STYLING =================
            using (var header = worksheet.Cells[1, 1, 1, 5])
            {
                header.Style.Font.Bold = true;
            }

            worksheet.Cells.AutoFitColumns();

            // ================= RETURN FILE =================
            var fileBytes = await package.GetAsByteArrayAsync();

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Users_Template.xlsx"
            );
        }
        [HttpDelete("delete-group-users")]
        public async Task<IActionResult> DeleteGroupUsers([FromBody] List<string> usernames)
        {
            var response = new
            {
                acceptedObjects = new List<string>(),
                errors = new List<string>()
            };

            try
            {
                if (usernames == null || !usernames.Any())
                    return BadRequest("Usernames list is empty.");

                foreach (var username in usernames)
                {
                    var user = await _userManager.FindByNameAsync(username);

                    if (user == null)
                    {
                        response.errors.Add($"User '{username}' not found.");
                        continue;
                    }

                    var result = await _userManager.DeleteAsync(user);

                    if (!result.Succeeded)
                    {
                        response.errors.Add($"User '{username}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        continue;
                    }

                    response.acceptedObjects.Add(username);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // ================= IMPORT USERS FROM EXCEL =================
        [HttpPost("import-users-excel")]
        public async Task<IActionResult> ImportUsersFromExcel(IFormFile excelFile)
        {
            var response = new
            {
                acceptedObjects = new List<object>(),
                errors = new List<string>()
            };

            try
            {
                if (excelFile == null || excelFile.Length == 0)
                    return BadRequest("Excel file is empty.");

                using var ms = new MemoryStream();
                await excelFile.CopyToAsync(ms);
                ms.Position = 0;

                using var package = new ExcelPackage(ms);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                    return BadRequest("Worksheet not found.");

                int rows = worksheet.Dimension?.Rows ?? 0;

                for (int r = 2; r <= rows; r++) // skip header
                {
                    var username = worksheet.Cells[r, 1].Text?.Trim();
                    var email = worksheet.Cells[r, 2].Text?.Trim();
                    var password = worksheet.Cells[r, 3].Text?.Trim();
                    var rights = worksheet.Cells[r, 4].Text?.Trim();
                    var taxId = worksheet.Cells[r, 5].Text?.Trim();
                    int userType = Convert.ToInt32(worksheet.Cells[r, 6].Value);
                    if (string.IsNullOrWhiteSpace(username))
                        continue;

                    var existingUser = await _userManager.FindByNameAsync(username);

                    if (existingUser != null)
                    {
                        response.errors.Add($"Row {r}: User {username} already exists.");
                        continue;
                    }

                    User user = new()
                    {
                        UserName = username,
                        Email = email,
                        Rights = rights,
                        registrationTaxid = taxId,
                        userType = (Type)userType
                    };
                    
                    var result = await _userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        response.errors.Add($"Row {r}: {string.Join(",", result.Errors.Select(e => e.Description))}");
                        continue;
                    }

                    response.acceptedObjects.Add(new
                    {
                        username,
                        email,
                        rights,
                        taxId,
                        userType
                    });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                Rights = model.Rights,
                allowance = model.allowance,
                userType = (Type)model.userType
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
                new Claim("allowance", user.allowance ?? ""),
                new Claim("userType", user.userType.ToString() ?? "0"),
                new Claim(ClaimTypes.NameIdentifier ,user.Id),
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
                expires: DateTime.Now.AddDays(300),
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
                u.registrationTaxid,
                u.userType
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
            user.allowance = model.allowance;
            user.userType = (Type)model.userType; 
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

    public record RegisterModel(string Username, string Email, string Password , string Rights , string allowance, int userType);
    public record LoginModel(string Username, string Password);
    public record UpdateRightsModel(string Username, string Rights , string allowance, int userType );
    public record UpdateUserByNameModel(string Id, string? Email, string? NewUsername, string? Rights );
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
    string? NewPassword,
    string? userType
);
}