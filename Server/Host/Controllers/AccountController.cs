using Application;
using Domain.Exceptions;
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
using System.Threading;
using System.Threading.Tasks;

namespace Host.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;
    private readonly AccountService _accountService;

    public AccountController(ILogger<AccountController> logger, IConfiguration configuration, AccountService accountService)
    {
        _logger = logger;
        _configuration = configuration;
        _accountService = accountService;
    }

    [HttpGet]
    public AccountService.FormRes GetForm()
    {
        return _accountService.GetForm();
    }

    [HttpPost]
    public async Task<ActionResult> RegisterAsync([FromBody] AccountService.RegisterReq body, CancellationToken ct)
    {
        AccountService.RegisterResult result;
        try
        {
            result = await _accountService.RegisterAsync(body.Id, body.Nickname, body.Password, ct);
        }
        catch (DomainException e)
        {
            return BadRequest(e.Code);
        }

        return result switch
        {
            AccountService.RegisterResult.Ok => Ok(),
            AccountService.RegisterResult.DuplicateId => Conflict(-1),
            AccountService.RegisterResult.DuplicateNickname => Conflict(-2),
            AccountService.RegisterResult.DuplicateAccount => Conflict(-3),
            _ => throw new NotImplementedException(),
        };
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
            issuer: Env.ISSUER,
            audience: Env.AUD,
            claims: claims,
            notBefore: null,
            expires: expires,
            issuedAt: issuedAt);

        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
