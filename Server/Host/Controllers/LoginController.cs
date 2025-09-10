using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Host.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public string Login()
    {
        return CreateAuthToken();
    }

    [Authorize("User")]
    [HttpGet("User")]
    public string User2()
    {
        return "User LoggedIn";
    }

    [Authorize("Admin")]
    [HttpGet("Admin")]
    public string Admin()
    {
        return "Admin LoggedIn";
    }

    private string CreateAuthToken()
    {
        IConfiguration configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        string issKey = configuration["ISS_KEY"]!;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim> claims = [
            new Claim(ClaimTypes.Role, "User"),
            ];

        var issuedAt = DateTime.UtcNow;
        var expires = issuedAt.AddHours(12);

        var header = new JwtHeader(credentials);
        var payload = new JwtPayload(
            issuer: "https://tradingking.com",
            audience: "tradingking-api",
            claims: claims,
            notBefore: null,
            expires: expires,
            issuedAt: issuedAt);

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
