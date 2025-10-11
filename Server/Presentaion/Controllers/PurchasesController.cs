using Application.Orchestrations;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Presentaion.Controllers;

[ApiController]
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

    [HttpGet("{userSeq}")]
    public Task GetAsync(int userSeq, CancellationToken ct)
    {
        return _purchaseService.GetAllAsync(userSeq, ct);
    }

    [HttpPost("buy")]
    public async Task<ActionResult> BuyAsync(PurchaseService.PurchaseReq req, CancellationToken ct)
    {
        try
        {
            await _purchaseService.BuyAsync(req, ct);
        }
        catch (DomainException e)
        {
            return Conflict(e.Code);
        }

        return Ok();
    }
}
