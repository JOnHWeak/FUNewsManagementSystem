using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using BusinessObjects;
using Microsoft.IdentityModel.Tokens;
using Services.DTO.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Services.DTO.Request;

namespace FUNewsManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public SystemAccountController(ISystemAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string adminEmail = _configuration["AdminAccount:Email"];
            string adminPassword = _configuration["AdminAccount:Password"];

            if (request.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
            {
                // Xác thực admin theo appsettings.json
                if (request.Password != adminPassword)
                    return Unauthorized("Incorrect password.");

                var claims = new[]
                {
            new Claim(ClaimTypes.Email, adminEmail),
            new Claim("Role", "0") // Role 0 cho admin
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);

                string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new LoginResponse
                {
                    Token = jwtToken,
                    Email = adminEmail,
                    Role = 0
                });
            }
            else
            {
                // Trường hợp người dùng bình thường, lấy từ DB
                var account = await _accountService.GetAccountByEmailAsync(request.Email);
                if (account == null)
                    return Unauthorized("Email does not exist.");

                if (account.AccountPassword != request.Password)
                    return Unauthorized("Incorrect password.");

                var claims = new[]
                {
            new Claim(ClaimTypes.Email, account.AccountEmail),
            new Claim("Role", account.AccountRole?.ToString() ?? "0")
        };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);

                string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new LoginResponse
                {
                    Token = jwtToken,
                    Email = account.AccountEmail,
                    Role = account.AccountRole ?? 0
                });
            }
        }


        [Authorize(Policy = "AdminOnly")] // Only admins can perform actions on SystemAccount
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(short id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        [Authorize(Policy = "AdminOnly")] // Only admins can perform actions on SystemAccount
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] SystemAccount account)
        {
            await _accountService.CreateAccountAsync(account);
            return CreatedAtAction(nameof(GetAccountById), new { id = account.AccountId }, account);
        }

        [Authorize(Policy = "AdminOnly")] // Only admins can perform actions on SystemAccount
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(short id, [FromBody] SystemAccount account)
        {
            if (id != account.AccountId)
                return BadRequest("The account ID in the URL does not match the request body.");

            await _accountService.UpdateAccountAsync(account);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")] // Only admins can perform actions on SystemAccount
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(short id)
        {
            // You can add logic to check if the account has associated news articles before deletion
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }
    }
}
