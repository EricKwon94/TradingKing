using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    private readonly IConfiguration _configuration;

    public LoginController(ILogger<LoginController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public string Login()
    {
        return CreateAuthToken();
    }

    [HttpGet("version")]
    public int Version()
    {
        int version = 0;
        _logger.LogInformation("{version} ø‰√ª", version);
        return version;
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
        string issKey = _configuration["ISS_KEY"]!;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(issKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        IEnumerable<Claim> claims = [
            new Claim(ClaimTypes.Role, "User"),
            ];

        var issuedAt = DateTime.UtcNow;
        var expires = issuedAt.AddHours(12);

        var header = new JwtHeader(credentials);
        var payload = new JwtPayload(
            issuer: Constant.ISSUER,
            audience: Constant.AUD,
            claims: claims,
            notBefore: null,
            expires: expires,
            issuedAt: issuedAt);

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
