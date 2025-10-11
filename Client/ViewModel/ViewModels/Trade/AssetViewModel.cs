using Application.Gateways;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels.Trade;

public partial class AssetViewModel : BaseViewModel
{
    private readonly ILogger<AssetViewModel> _logger;
    private readonly IAlertService _alert;
    private readonly IExchangeApi _exchangeApi;
    private readonly IExchangeTickerApi _tickerApi;
    private readonly IDispatcher _dispatcher;

    private readonly List<IExchangeApi.MarketRes> _markets = [];

    [ObservableProperty]
    private long _availableCash = 100_000_000;

    [ObservableProperty]
    private long _coinCash;

    public ObservableCollection<MyAsset> Purchases { get; } = [];

    public AssetViewModel(
        ILogger<AssetViewModel> logger, IAlertService alert, IDispatcher dispatcher,
        IExchangeApi exchangeApi, IExchangeTickerApi tickerApi)
    {
        _logger = logger;
        _alert = alert;
        _dispatcher = dispatcher;
        _exchangeApi = exchangeApi;
        _tickerApi = tickerApi;
    }

    public override async Task LoadAsync(CancellationToken ct)
    {
        IEnumerable<IExchangeApi.MarketRes>? markets = null;
        try
        {
            markets = await _exchangeApi.GetMarketsAsync(ct);
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", ct);
        }

        if (markets != null)
        {
            foreach (var market in markets.Where(c => c.market.StartsWith("KRW-")))
            {
                _markets.Add(market);
            }
        }
    }

    public override async Task OnAppearingAsync(CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return;

        IEnumerable<MyAsset> purchases = [
            new MyAsset(code:"KRW-BTC", name: "", totalQuantity: 1, avgPrice: 177_120_000),
            new MyAsset(code:"KRW-BTC", name:"", totalQuantity: 2, avgPrice: 177_020_000),
            new MyAsset(code:"KRW-DOGE", name:"", totalQuantity: 10, avgPrice: 300),
            new MyAsset(code:"KRW-DOGE", name:"", totalQuantity: 3, avgPrice: 400),
            new MyAsset(code:"KRW-MBL", name:"", totalQuantity:  500.5, avgPrice: 2.905),
            ];

        var grouped = purchases
            .GroupBy(p => p.Code)
            .Select(g =>
            {
                double totalQuantity = g.Sum(p => p.TotalQuantity);
                double totalPrice = g.Sum(p => p.AvgPrice * p.TotalQuantity);
                double avgPrice = totalPrice / totalQuantity;
                string name = _markets.Single(e => e.market == g.Key).korean_name;

                return new MyAsset(g.Key, name, totalQuantity, avgPrice)
                {
                    TotalPrice = totalPrice,
                };
            });

        foreach (var purchse in grouped)
        {
            Purchases.Add(purchse);
        }

        try
        {
            await _tickerApi.ConnectAsync(ct);
            await _tickerApi.SendAsync(grouped.Select(x => x.Code), ct);
            IsBusy = false;
            await foreach (var item in _tickerApi.ReceiveAsync(ct))
            {
                MyAsset asset = Purchases.Single(e => e.Code == item.code);
                asset.EvaluationPrice = asset.TotalQuantity * item.trade_price;

                CoinCash = Convert.ToInt64(Purchases.Sum(x => x.EvaluationPrice));
            }
        }
        catch (OperationCanceledException)
        {
            _tickerApi.Dispose();
        }
    }

    public override void OnDisappearing()
    {
        Purchases.Clear();
    }
}