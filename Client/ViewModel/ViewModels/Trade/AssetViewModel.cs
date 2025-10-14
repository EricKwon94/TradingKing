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
    private readonly IOrderApi _orderApi;

    private readonly List<IExchangeApi.MarketRes> _markets = [];

    [ObservableProperty]
    private long _availableCash = 100_000_000;

    [ObservableProperty]
    private long _coinCash;

    public ObservableCollection<MyAssetViewModel> Purchases { get; } = [];

    public AssetViewModel(
        ILogger<AssetViewModel> logger, IAlertService alert, IDispatcher dispatcher,
        IExchangeApi exchangeApi, IExchangeTickerApi tickerApi, IOrderApi orderApi)
    {
        _logger = logger;
        _alert = alert;
        _dispatcher = dispatcher;
        _exchangeApi = exchangeApi;
        _tickerApi = tickerApi;
        _orderApi = orderApi;
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

        IEnumerable<IOrderApi.OrderRes> orders = await _orderApi.GetAllAsync(ct);
        AvailableCash = Convert.ToInt64(orders.Where(e => e.Code == "KRW-CASH").Sum(e => e.Price));

        var grouped = orders
            .Where(e => e.Code != "KRW-CASH")
            .GroupBy(p => p.Code)
            .Select(g =>
            {
                double totalBuyPrice = g.Where(e => e.Quantity > 0).Sum(e => e.Price * e.Quantity);
                double totalSellPrice = -1 * g.Where(e => e.Quantity < 0).Sum(e => e.Price * e.Quantity);

                double totalQuantity = g.Sum(p => p.Quantity);
                double totalPrice = -1 * g.Sum(p => p.Price * p.Quantity);
                string name = _markets.Single(e => e.market == g.Key).korean_name;

                return new MyAssetViewModel(
                    code: g.Key, name,
                    totalQuantity, totalBuyPrice, totalSellPrice)
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
                MyAssetViewModel asset = Purchases.Single(e => e.Code == item.code);
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