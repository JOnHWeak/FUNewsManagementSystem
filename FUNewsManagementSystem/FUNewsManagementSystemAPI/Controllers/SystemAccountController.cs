using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.DTO.Request;
using Services.DTO.Response;

namespace FUNewsManagementSystemAPI.Controllers
{
    [Route("odata/SystemAccounts")]
    public class SystemAccountController : ODataController
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public SystemAccountController(
            ISystemAccountService accountService,
            IConfiguration configuration
        )
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        // Keep login as regular API endpoint since it's not a typical OData operation
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body is required");
                }

                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest("Email and password are required");
                }

                string adminEmail = _configuration["AdminAccount:Email"];
                string adminPassword = _configuration["AdminAccount:Password"];

                if (request.Email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
                {
                    // Admin authentication from appsettings.json
                    if (request.Password != adminPassword)
                        return Unauthorized("Incorrect password");

                    var adminClaims = new[]
                    {
                        new Claim(ClaimTypes.Email, adminEmail),
                        new Claim("Role", "0"),
                        new Claim(ClaimTypes.NameIdentifier, "AccountId"),
                        new Claim(ClaimTypes.Name, "Administrator"),
                    };

                    var adminToken = GenerateJwtToken(adminClaims);

                    return Ok(
                        new LoginResponse
                        {
                            Token = adminToken,
                            Email = adminEmail,
                            Role = 0,
                        }
                    );
                }
                else
                {
                    // Regular user authentication from database
                    var account = await _accountService.GetAccountByEmailAsync(request.Email);
                    if (account == null)
                        return Unauthorized("Email does not exist");

                    if (account.AccountPassword != request.Password)
                        return Unauthorized("Incorrect password");

                    var userClaims = new[]
                    {
                        new Claim(ClaimTypes.Email, account.AccountEmail),
                        new Claim("Role", account.AccountRole?.ToString() ?? "0"),
                        new Claim(ClaimTypes.Name, account.AccountName ?? account.AccountEmail),
                    };

                    var userToken = GenerateJwtToken(userClaims);

                    return Ok(
                        new LoginResponse
                        {
                            Token = userToken,
                            Email = account.AccountEmail,
                            Role = account.AccountRole ?? 0,
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new { error = "Internal server error", message = ex.Message }
                );
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetMe()
        {
            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email);
                var roleClaim = User.FindFirst("Role");
                var nameClaim = User.FindFirst(ClaimTypes.Name);
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (emailClaim == null)
                    return Unauthorized();

                var role = int.Parse(roleClaim?.Value ?? "0");

                return Ok(
                    new
                    {
                        Email = emailClaim.Value,
                        Name = nameClaim?.Value ?? emailClaim.Value,
                        Role = role,
                        RoleName = GetRoleName(role),
                        Id = id ?? "Unknown",
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new { error = "Error getting user info", message = ex.Message }
                );
            }
        }

        private string GenerateJwtToken(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GetRoleName(int role)
        {
            return role switch
            {
                0 => "Administrator",
                1 => "Staff",
                2 => "Lecturer",
                _ => "Unknown",
            };
        }

        [Authorize(Policy = "AdminOnly")]
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get([FromODataUri] short key)
        {
            var account = await _accountService.GetAccountByIdAsync(key);
            if (account == null)
                return NotFound();
            return Ok(account);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _accountService.CreateAccountAsync(account);
            return Created(account);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{key}")]
        public async Task<IActionResult> Put(
            [FromODataUri] short key,
            [FromBody] SystemAccount account
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (key != account.AccountId)
                return BadRequest("The account ID in the URL does not match the request body.");

            await _accountService.UpdateAccountAsync(account);
            return Updated(account);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{key}")]
        public async Task<IActionResult> Patch(
            [FromODataUri] short key,
            [FromBody] Delta<SystemAccount> delta
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _accountService.GetAccountByIdAsync(key);
            if (existing == null)
                return NotFound();

            delta.Put(existing);
            await _accountService.UpdateAccountAsync(existing);
            return Updated(existing);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] short key)
        {
            var account = await _accountService.GetAccountByIdAsync(key);
            if (account == null)
                return NotFound();

            await _accountService.DeleteAccountAsync(key);
            return NoContent();
        }
    }
}
