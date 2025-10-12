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
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly OrderService _orderService;

    public OrdersController(ILogger<OrdersController> logger, OrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpGet("policy")]
    public int GetPolicy()
    {
        return _orderService.GetPolicy();
    }

    [HttpGet]
    public Task<IEnumerable<OrderService.OrderRes>> GetAsync(CancellationToken ct)
    {
        int userSeq = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return _orderService.GetAllAsync(userSeq, ct);
    }

    [HttpPost("buy")]
    public async Task<ActionResult> BuyAsync(OrderService.OrderReq req, CancellationToken ct)
    {
        int userSeq = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await _orderService.BuyAsync(userSeq, req, ct);
        }
        catch (DomainException e)
        {
            return Conflict(e.Code);
        }

        return Ok();
    }
}
