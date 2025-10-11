using Application.Orchestrations;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Presentaion.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly ILogger<PurchasesController> _logger;
    private readonly PurchaseService _purchaseService;

    public PurchasesController(ILogger<PurchasesController> logger, PurchaseService purchaseService)
    {
        _logger = logger;
        _purchaseService = purchaseService;
    }

    [HttpGet]
    public Task<IEnumerable<PurchaseService.PurchaseRes>> GetAsync(CancellationToken ct)
    {
        int userSeq = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return _purchaseService.GetAllAsync(userSeq, ct);
    }

    [HttpPost("buy")]
    public async Task<ActionResult> BuyAsync(PurchaseService.PurchaseReq req, CancellationToken ct)
    {
        int userSeq = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await _purchaseService.BuyAsync(userSeq, req, ct);
        }
        catch (DomainException e)
        {
            return Conflict(e.Code);
        }

        return Ok();
    }
}
