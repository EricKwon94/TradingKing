using Application.Orchestrations;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAsync([FromBody] AccountService.RegisterReq body, CancellationToken ct)
    {
        AccountService.RegisterResult result;
        try
        {
            result = await _accountService.RegisterAsync(body.Id, body.Password, ct);
        }
        catch (DomainException e)
        {
            return BadRequest(e.Code);
        }

        return result switch
        {
            AccountService.RegisterResult.Ok => Ok(),
            AccountService.RegisterResult.DuplicateId => Conflict(-1),
            AccountService.RegisterResult.DuplicateAccount => Conflict(-2),
            _ => throw new NotImplementedException(),
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromBody] AccountService.LoginReq body, CancellationToken ct)
    {
        string issKey = _configuration["ISS_KEY"]!;

        string? jwt = await _accountService.LoginAsync(body.Id, body.Password, issKey, Env.ISSUER, Env.AUD, ct);
        if (string.IsNullOrEmpty(jwt))
            return NotFound();

        return jwt;
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
}
