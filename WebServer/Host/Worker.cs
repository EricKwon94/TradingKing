using Application.Gateways;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presentaion;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Host;

public class Worker : BackgroundService
{
    private const int REFRESH_DELAY_SECOND = 1;

    private readonly ILogger<Worker> _logger;
    private readonly IExchangeApi _exchangeApi;
    private readonly ConcurrentDictionary<string, double> _cache;
    private readonly TradingKingContext _context;
    private readonly ChannelReader<Domain.Order> _reader;

    private readonly ConcurrentDictionary<string, double> _userAssets = [];
    private readonly ConcurrentDictionary<Guid, Domain.Order> _orders = [];

    private int _seasonId = 1;
    private HashSet<string> _codes = null!;

    public Worker(ILogger<Worker> logger, IConfiguration config, IExchangeApi exchangeApi,
        [FromKeyedServices(Constant.CACHE_KEY)] ConcurrentDictionary<string, double> cache,
        ChannelReader<Domain.Order> reader)
    {
        _logger = logger;

        var builder = new DbContextOptionsBuilder<TradingKingContext>()
            .UseSqlServer(config.GetConnectionString("TradingKing"));

        _context = new TradingKingContext(builder.Options);
        _exchangeApi = exchangeApi;
        _cache = cache;
        _reader = reader;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await InitAsync(ct);

        Task task1 = AggregateRankAsync(ct);
        Task task2 = ReceiveOrderMessageAsync(ct);
        //Task task3 = ReceiveSeasonMessageAsync(ct);

        await Task.WhenAll(task1, task2);
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
        _codes.Remove(Domain.Order.DEFAULT_CODE);

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
        await foreach (Domain.Order order in _reader.ReadAllAsync(ct))
        {
            if (order.SeasonId == _seasonId && _orders.TryAdd(order.Id, order))
            {
                // INSERT
                _logger.LogInformation("{order}", order);

                if (order.Code != Domain.Order.DEFAULT_CODE)
                    _codes.Add(order.Code);

                string key = $"{order.UserId}|{order.Code}";
                _userAssets.AddOrUpdate(key, order.Quantity, (key, prev) =>
                {
                    return prev + order.Quantity;
                });
            }
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

            foreach (var asset in totalAssets)
            {
                _cache.AddOrUpdate(asset.Key, asset.Value, (key, prevValue) => asset.Value);
            }
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

            if (code == Domain.Order.DEFAULT_CODE)
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
