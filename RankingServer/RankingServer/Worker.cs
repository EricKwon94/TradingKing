using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Shared;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RankingServer;

internal class Worker : BackgroundService
{
    private const string DEFAULT_CASH = "KRW-CASH";
    private const int REFRESH_DELAY_SECOND = 1;

    private readonly ILogger<Worker> _logger;
    private readonly TradingKingContext _context;
    private readonly IDatabase _redis;
    private readonly ServiceBusReceiver _orderReceiver;
    private readonly ServiceBusReceiver _rankReceiver;
    private readonly IExchangeApi _exchangeApi;

    private ConcurrentDictionary<string, double> _userAssets = null!;
    private HashSet<OrderModel> _orders = null!;
    private HashSet<string> _codes = null!;

    public Worker(
        ILogger<Worker> logger, TradingKingContext context, IDatabase redis,
        [FromKeyedServices("order")] ServiceBusReceiver orderReceiver,
        [FromKeyedServices("rank")] ServiceBusReceiver rankReceiver,
        IExchangeApi exchangeApi)
    {
        _logger = logger;
        _context = context;
        _redis = redis;
        _orderReceiver = orderReceiver;
        _rankReceiver = rankReceiver;
        _exchangeApi = exchangeApi;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await InitAsync(ct);

        Task task1 = AggregateRankAsync(ct);
        Task task2 = ReceiveOrderMessageAsync(ct);

        await Task.WhenAll(task1, task2);
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

    private async Task ReceiveOrderMessageAsync(CancellationToken ct)
    {
        await foreach (ServiceBusReceivedMessage item in _orderReceiver.ReceiveMessagesAsync(ct))
        {
            await _orderReceiver.CompleteMessageAsync(item, ct);

            var order = JsonSerializer.Deserialize<OrderModel>(item.Body)!;

            if (_orders.Add(order))
            {
                // INSERT
                _logger.LogInformation("{order}", order);

                if (order.Code != DEFAULT_CASH)
                    _codes.Add(order.Code);

                string key = $"{order.UserId}|{order.Code}";
                _userAssets.AddOrUpdate(key, order.Quantity, (key, prev) =>
                {
                    return prev + order.Quantity;
                });
            }
            else if (_orders.TryGetValue(order, out var actualOrder)
                && string.IsNullOrEmpty(order.UserId)
                && string.IsNullOrEmpty(order.Code))
            {
                // DELETE
                string key = $"{actualOrder.UserId}|{actualOrder.Code}";
                if (_orders.Remove(actualOrder) && _userAssets.TryGetValue(key, out double prev))
                {
                    _userAssets.TryUpdate(key, prev - actualOrder.Quantity, prev);
                }
            }
        }
    }

    private async Task AggregateRankAsync(CancellationToken ct)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(REFRESH_DELAY_SECOND));
        while (await timer.WaitForNextTickAsync(ct))
        {
            IEnumerable<IExchangeApi.TickerRes> tickers;
            try
            {
                tickers = await _exchangeApi.GetTickerAsync(_codes, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
                continue;
            }

            Dictionary<string, double> totalAssets = CalculaterTotalAsset(tickers);

            int i = 0;
            SortedSetEntry[] entries = new SortedSetEntry[totalAssets.Count];
            foreach (var asset in totalAssets)
            {
                entries[i++] = new SortedSetEntry(asset.Key, asset.Value);
            }
            await _redis.SortedSetAddAsync("ranking", entries);
        }
    }

    private Dictionary<string, double> CalculaterTotalAsset(IEnumerable<IExchangeApi.TickerRes> tickers)
    {
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
        return totalAssets;
    }
}