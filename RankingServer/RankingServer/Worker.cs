using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Text.Json;

namespace RankingServer;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TradingKingContext _context;
    private readonly ServiceBusReceiver _orderReceiver;
    private readonly ServiceBusReceiver _rankReceiver;

    public Worker(
        ILogger<Worker> logger, TradingKingContext context,
        [FromKeyedServices("order")] ServiceBusReceiver orderReceiver, [FromKeyedServices("rank")] ServiceBusReceiver rankReceiver)
    {
        _logger = logger;
        _context = context;
        _orderReceiver = orderReceiver;
        _rankReceiver = rankReceiver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var orders = await _context.Orders.AsNoTracking().ToListAsync(stoppingToken);
        await foreach (ServiceBusReceivedMessage item in _orderReceiver.ReceiveMessagesAsync(stoppingToken))
        {
            var order = JsonSerializer.Deserialize<OrderModel>(item.Body);
            _logger.LogInformation("{order}", order);
            await _orderReceiver.CompleteMessageAsync(item, stoppingToken);
        }
    }
}
