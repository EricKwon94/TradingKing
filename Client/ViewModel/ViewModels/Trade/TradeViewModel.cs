using Application.Gateways;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ViewModel.Contracts;
using ViewModel.ViewModels.Trade;

namespace ViewModel.ViewModels;

public partial class TradeViewModel : BaseViewModel, IQueryAttributable
{
    private const int MIN_ORDER_PRICE = 1000;

    private readonly ILogger<TradeViewModel> _logger;
    private readonly IAlertService _alert;
    private readonly ICryptoService _cryptoService;
    private readonly ICryptoTickerService _cryptoTickerService;
    private readonly IDispatcher _dispatcher;

    private IEnumerable<ICryptoService.CryptoAsset>? _cryptoAssets;

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
        ICryptoService cryptoService, ICryptoTickerService cryptoTickerService)
    {
        _logger = logger;
        _alert = alert;
        _dispatcher = dispatcher;
        _cryptoService = cryptoService;
        _cryptoTickerService = cryptoTickerService;
    }

    [RelayCommand(CanExecute = nameof(CanSearch))]
    public async Task SearchAsync(CancellationToken ct)
    {
        if (_cryptoAssets == null)
            return;

        var asset = _cryptoAssets.Single(x => x.korean_name == SearchText);

        if (SelectedTicker?.Code == asset.market)
        {
            SearchText = null;
            SelectedTicker = null;
            return;
        }

        try
        {
            await _cryptoTickerService.ConnectAsync(ct);
            await _cryptoTickerService.SendAsync([asset.market], ct);
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

    }

    [RelayCommand(CanExecute = nameof(CanSell))]
    public async Task SellAsync(CancellationToken ct)
    {

    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {

    }

    public override async void Initialize()
    {
        IsBusy = true;
        IEnumerable<ICryptoService.CryptoAsset>? cryptos = null;
        try
        {
            cryptos = await _cryptoService.GetAssetsAsync(default);
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", default);
        }

        if (cryptos != null)
        {
            _cryptoAssets = cryptos.Where(c => c.market.Contains("KRW"));
        }
        IsBusy = false;
    }

    private bool CanSearch()
    {
        return _cryptoAssets != null && _cryptoAssets.Any(x => x.korean_name == SearchText);
    }

    private bool CanBuy()
    {
        if (IsOrderByQuantity)
            return FinalCount >= MIN_ORDER_PRICE;
        else
            return int.TryParse(OrderCount, out var d) && d >= MIN_ORDER_PRICE;
    }

    private bool CanSell()
    {
        if (IsOrderByQuantity)
            return FinalCount >= MIN_ORDER_PRICE;
        else
            return int.TryParse(OrderCount, out var d) && d >= MIN_ORDER_PRICE;
    }

    private async void ReceiveTickerAsync(object? sender)
    {
        try
        {
            CancellationToken ct = (CancellationToken)sender!;
            await foreach (var ticker in _cryptoTickerService.ReceiveAsync(ct))
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
            _cryptoTickerService.Dispose();
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


