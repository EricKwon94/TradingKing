using Application.Gateways;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ViewModel.Contracts;

namespace ViewModel.ViewModels.Trade;

public partial class TradeViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ILogger<TradeViewModel> _logger;
    private readonly IAlertService _alert;
    private readonly IExchangeApi _exchangeApi;
    private readonly IExchangeTickerApi _tickerApi;
    private readonly IOrderApi _orderApi;
    private readonly IDispatcher _dispatcher;

    private readonly List<IExchangeApi.MarketRes> _markets = [];

    private int? _minOrderPrice;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private string? _searchText;

    [ObservableProperty]
    private TickerViewModel? _selectedTicker;

    private bool _isOrderByQuantity = true;
    public bool IsOrderByQuantity
    {
        get => _isOrderByQuantity;
        set
        {
            SetProperty(ref _isOrderByQuantity, value);
            OrderCount = "0";
        }
    }

    private string _orderCount = "0";
    public string OrderCount
    {
        get => _orderCount;
        set
        {
            SetProperty(ref _orderCount, value);
            if (SelectedTicker != null)
                CalculateFinalCount(this, SelectedTicker.Price);
            BuyCommand.NotifyCanExecuteChanged();
            SellCommand.NotifyCanExecuteChanged();
        }
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BuyCommand), nameof(SellCommand))]
    private double _finalCount;

    public TradeViewModel(
        ILogger<TradeViewModel> logger, IAlertService alert, IDispatcher dispatcher,
        IExchangeApi exchangeApi, IExchangeTickerApi tickerApi, IOrderApi oderApi)
    {
        _logger = logger;
        _alert = alert;
        _dispatcher = dispatcher;
        _exchangeApi = exchangeApi;
        _tickerApi = tickerApi;
        _orderApi = oderApi;
    }

    public override async Task LoadAsync(CancellationToken ct)
    {
        IEnumerable<IExchangeApi.MarketRes>? markets = null;
        try
        {
            markets = await _exchangeApi.GetMarketsAsync(ct);
            _minOrderPrice = await _orderApi.GetPolicyAsync(ct);
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

    [RelayCommand(CanExecute = nameof(CanSearch))]
    public async Task SearchAsync(CancellationToken ct)
    {
        var asset = _markets.Single(x => x.korean_name == SearchText);

        if (SelectedTicker?.Code == asset.market)
        {
            SearchText = null;
            SelectedTicker = null;
            return;
        }

        try
        {
            await _tickerApi.ConnectAsync(ct);
            await _tickerApi.SendAsync([asset.market], ct);
        }
        catch (Exception ex)
        {
            await _alert.DisplayAlertAsync("error", ex.Message, "ok", ct);
            SearchText = null;
            SelectedTicker = null;
            return;
        }

        OrderCount = "0";
        ThreadPool.QueueUserWorkItem(ReceiveTickerAsync, ct);
    }

    [RelayCommand(CanExecute = nameof(CanBuy))]
    public async Task BuyAsync(CancellationToken ct)
    {
        double quantity = IsOrderByQuantity ? double.Parse(OrderCount) : FinalCount;
        var req = new IOrderApi.OrderReq(SelectedTicker!.Code, quantity);

        try
        {
            await _orderApi.BuyAsync(req, ct);
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.Conflict && int.TryParse(e.Content, out int errorCode))
        {
            if (errorCode == -3)
                await _alert.DisplayAlertAsync("error", "돈이 부족해", "ok", ct);
            else if (errorCode == -4)
                await _alert.DisplayAlertAsync("error", $"최소 주문 금액 {_minOrderPrice!.Value}", "ok", ct);
            return;
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("error", e.Message, "ok", ct);
            return;
        }

        await _alert.DisplayAlertAsync("구매", "성공 o((>ω< ))o", "ok", ct);
    }

    [RelayCommand(CanExecute = nameof(CanSell))]
    public async Task SellAsync(CancellationToken ct)
    {

    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {

    }

    private bool CanSearch()
    {
        return _markets.Any(x => x.korean_name == SearchText);
    }

    private bool CanBuy()
    {
        if (IsOrderByQuantity)
            return double.TryParse(OrderCount, out var d) && d > 0 && FinalCount >= _minOrderPrice;
        else
            return int.TryParse(OrderCount, out var d) && d >= _minOrderPrice;
    }

    private bool CanSell()
    {
        if (IsOrderByQuantity)
            return double.TryParse(OrderCount, out var d) && d > 0 && FinalCount >= _minOrderPrice;
        else
            return int.TryParse(OrderCount, out var d) && d >= _minOrderPrice;
    }

    private async void ReceiveTickerAsync(object? sender)
    {
        try
        {
            CancellationToken ct = (CancellationToken)sender!;
            await foreach (var ticker in _tickerApi.ReceiveAsync(ct))
            {
                _dispatcher.Invoke(() =>
                {
                    if (SelectedTicker == null)
                    {
                        SelectedTicker = new TickerViewModel(ticker.code, ticker.trade_price, TickerViewModel.Change.Base);
                        SelectedTicker.PriceChanged += CalculateFinalCount;
                    }
                    else
                    {
                        SelectedTicker.Price = ticker.trade_price;
                        if (SelectedTicker.Code != ticker.code)
                        {
                            SelectedTicker.Code = ticker.code;
                            SelectedTicker.Change2 = TickerViewModel.Change.Base;
                        }
                    }
                });
            }
        }
        catch (OperationCanceledException)
        {
            _tickerApi.Dispose();
        }
    }

    private void CalculateFinalCount(object? sender, double tickerPrice)
    {
        if (IsOrderByQuantity && double.TryParse(OrderCount, out var quantity))
            FinalCount = tickerPrice * quantity;
        else if (!IsOrderByQuantity && int.TryParse(OrderCount, out var price))
            FinalCount = price / tickerPrice;
    }
}


