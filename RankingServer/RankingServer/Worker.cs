using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RankingServer;

internal class Worker : BackgroundService
{
    private const string DEFAULT_CASH = "KRW-CASH";
    private const int REFRESH_DELAY = 1000;

    private readonly ILogger<Worker> _logger;
    private readonly TradingKingContext _context;
    private readonly ServiceBusReceiver _orderReceiver;
    private readonly ServiceBusReceiver _rankReceiver;
    private readonly IExchangeApi _exchangeApi;

    private ConcurrentDictionary<string, double> _userAssets = null!;
    private HashSet<OrderModel> _orders = null!;
    private HashSet<string> _codes = null!;

    public Worker(
        ILogger<Worker> logger, TradingKingContext context,
        [FromKeyedServices("order")] ServiceBusReceiver orderReceiver,
        [FromKeyedServices("rank")] ServiceBusReceiver rankReceiver,
        IExchangeApi exchangeApi)
    {
        _logger = logger;
        _context = context;
        _orderReceiver = orderReceiver;
        _rankReceiver = rankReceiver;
        _exchangeApi = exchangeApi;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await InitAsync(ct);

        Timer timer = new(TimerElapsed, ct, 0, REFRESH_DELAY);

        await foreach (ServiceBusReceivedMessage item in _orderReceiver.ReceiveMessagesAsync(ct))
        {
            var order = JsonSerializer.Deserialize<OrderModel>(item.Body)!;
            _logger.LogInformation("{order}", order);
            await _orderReceiver.CompleteMessageAsync(item, ct);

            if (_orders.Add(order))
            {
                if (order.Code != DEFAULT_CASH)
                    _codes.Add(order.Code);

                string key = $"{order.UserId}|{order.Code}";
                _userAssets.AddOrUpdate(key, order.Quantity, (key, prev) =>
                {
                    return prev + order.Quantity;
                });
            }
        }
    }

    private async void TimerElapsed(object? state)
    {
        CancellationToken ct = (CancellationToken)state!;

        var tickers = await _exchangeApi.GetTickerAsync(_codes, ct);

        Dictionary<string, double> totalAssets = [];

        foreach (var item in _userAssets)
        {
            string[] split = item.Key.Split('|');
            string userId = split[0];
            string code = split[1];
            double price = 0;

            if (code == DEFAULT_CASH)
            {
                price = item.Value;
            }
            else
            {
                var ticker = tickers.Single(e => e.market == code);
                price = item.Value * ticker.trade_price;
            }

            if (!totalAssets.TryAdd(userId, price))
            {
                double money = totalAssets[userId];
                totalAssets[userId] = money + price;
            }
        }

        foreach (var v in totalAssets)
        {
            Console.WriteLine($"{v.Key}: {v.Value}");
        }
        Console.WriteLine();
    }

    private async Task InitAsync(CancellationToken ct)
    {
        _orders = await _context.Orders
            .AsNoTracking()
            .ToHashSetAsync(new OrderModel.Comparer(), ct);

        _codes = _orders
            .Select(o => o.Code)
            .ToHashSet();
        _codes.Remove(DEFAULT_CASH);

        Dictionary<string, double> userAssets = _orders
            .GroupBy(e => new { e.UserId, e.Code })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.Code,
                Quantity = g.Sum(g => g.Quantity)
            })
            .ToDictionary(
            keySelector: g => $"{g.UserId}|{g.Code}",
            elementSelector: g => g.Quantity);

        _userAssets = new ConcurrentDictionary<string, double>(userAssets);
    }
}