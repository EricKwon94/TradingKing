using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Shared;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace RankingServer;

internal class Worker : BackgroundService
{
    private const string DEFAULT_CODE = "KRW-CASH";
    private const int DEFAULT_QUANTITY = 100_000_000;
    private const int REFRESH_DELAY_SECOND = 1;
    private const string RANKING_KEY = "ranking";

    private readonly ILogger<Worker> _logger;
    private readonly TradingKingContext _context;
    private readonly IDatabase _redis;
    private readonly ServiceBusReceiver _orderReceiver;
    private readonly ServiceBusReceiver _seasonReceiver;
    private readonly IExchangeApi _exchangeApi;

    private readonly ConcurrentDictionary<string, double> _userAssets = [];
    private readonly ConcurrentDictionary<Guid, OrderModel> _orders = [];

    private int _seasonId = 1;
    private HashSet<string> _codes = null!;

    public Worker(
        ILogger<Worker> logger, TradingKingContext context, IDatabase redis,
        [FromKeyedServices("order")] ServiceBusReceiver orderReceiver,
        [FromKeyedServices("rank")] ServiceBusReceiver seasonReceiver,
        IExchangeApi exchangeApi)
    {
        _logger = logger;
        _context = context;
        _redis = redis;
        _orderReceiver = orderReceiver;
        _seasonReceiver = seasonReceiver;
        _exchangeApi = exchangeApi;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await InitAsync(ct);

        Task task1 = AggregateRankAsync(ct);
        Task task2 = ReceiveOrderMessageAsync(ct);
        Task task3 = ReceiveSeasonMessageAsync(ct);

        await Task.WhenAll(task1, task2, task3);
    }

    private async Task InitAsync(CancellationToken ct)
    {
        _seasonId = await _context.Seasons
            .AsNoTracking()
            .MaxAsync(x => x.Id, ct);

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(e => e.SeasonId == _seasonId)
            .ToListAsync(ct);

        _codes = orders
            .Select(o => o.Code)
            .ToHashSet();
        _codes.Remove(DEFAULT_CODE);

        _orders.Clear();
        foreach (var order in orders)
        {
            _orders.TryAdd(order.Id, order);
        }

        Dictionary<string, double> userAssets = _orders
            .GroupBy(e => new { e.Value.UserId, e.Value.Code })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.Code,
                Quantity = g.Sum(g => g.Value.Quantity)
            })
            .ToDictionary(
            keySelector: g => $"{g.UserId}|{g.Code}",
            elementSelector: g => g.Quantity);

        _userAssets.Clear();
        foreach (var asset in userAssets)
        {
            _userAssets.TryAdd(asset.Key, asset.Value);
        }
    }

    private async Task ReceiveOrderMessageAsync(CancellationToken ct)
    {
        await foreach (ServiceBusReceivedMessage item in _orderReceiver.ReceiveMessagesAsync(ct))
        {
            await _orderReceiver.CompleteMessageAsync(item, ct);

            var orderModel = JsonSerializer.Deserialize<OrderModel>(item.Body)!;

            if (orderModel.SeasonId == _seasonId && _orders.TryAdd(orderModel.Id, orderModel))
            {
                // INSERT
                _logger.LogInformation("{order}", orderModel);

                if (orderModel.Code != DEFAULT_CODE)
                    _codes.Add(orderModel.Code);

                string key = $"{orderModel.UserId}|{orderModel.Code}";
                _userAssets.AddOrUpdate(key, orderModel.Quantity, (key, prev) =>
                {
                    return prev + orderModel.Quantity;
                });
            }
            else if (_orders.TryGetValue(orderModel.Id, out var order)
                && string.IsNullOrEmpty(orderModel.UserId) && string.IsNullOrEmpty(orderModel.Code))
            {
                // DELETE
                string key = $"{order.UserId}|{order.Code}";
                if (_orders.TryRemove(order.Id, out var _) && _userAssets.TryGetValue(key, out double prev))
                {
                    _userAssets.TryUpdate(key, prev - order.Quantity, prev);
                }
            }
        }
    }

    private async Task ReceiveSeasonMessageAsync(CancellationToken ct)
    {
        await foreach (ServiceBusReceivedMessage item in _seasonReceiver.ReceiveMessagesAsync(ct))
        {
            await _seasonReceiver.CompleteMessageAsync(item, ct);
            var season = JsonSerializer.Deserialize<SeasonModel>(item.Body)!;
            _logger.LogInformation("Season Changed {id}", season.Id);

            await InitAsync(ct);
            SortedSetEntry[] entries = await _redis.SortedSetRangeByRankWithScoresAsync(RANKING_KEY, 0, -1, Order.Descending);
            await _redis.KeyDeleteAsync(RANKING_KEY);
            // TODO: 명예의전당등록

            // 모든 유저 1억 지급
            List<string> users = await _context.Users
                .AsNoTracking().Select(e => e.Id).ToListAsync(ct);

            foreach (var user in users)
            {
                var order = new OrderModel(Guid.NewGuid(), _seasonId, user, DEFAULT_CODE, DEFAULT_QUANTITY, 1);
                await _context.Orders.AddAsync(order, ct);
            }
            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task AggregateRankAsync(CancellationToken ct)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(REFRESH_DELAY_SECOND));
        while (await timer.WaitForNextTickAsync(ct))
        {
            IEnumerable<IExchangeApi.TickerRes> tickers = [];
            try
            {
                if (_codes.Count > 0)
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
            await _redis.SortedSetAddAsync(RANKING_KEY, entries);
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

            if (code == DEFAULT_CODE)
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