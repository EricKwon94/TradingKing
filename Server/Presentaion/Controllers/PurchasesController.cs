using Application.Orchestrations;
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
        return _purchaseService.GetAsync(userSeq, ct);
    }
}
