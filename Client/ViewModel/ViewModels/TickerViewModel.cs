using Application.Gateways;
using Application.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModel.Contracts;

namespace ViewModel.ViewModels;

public class TickerViewModel : BaseViewModel, IQueryAttributable
{
    private readonly ILogger<TickerViewModel> _logger;
    private readonly IAlertService _alert;
    private readonly ICryptoService _cryptoService;
    private readonly ICryptoTickerService _cryptoWsService;

    public TickerViewModel(
        ILogger<TickerViewModel> logger, IAlertService alert,
        ICryptoService cryptoService, ICryptoTickerService cryptoWsService)
    {
        _logger = logger;
        _alert = alert;
        _cryptoService = cryptoService;
        _cryptoWsService = cryptoWsService;
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
            cryptos = cryptos.Where(c => c.market.Contains("KRW"));
        }

        await _cryptoWsService.ConnectAsync(default);
        await _cryptoWsService.SendAsync(["KRW-BTC"], default);

        await foreach (var ticker in _cryptoWsService.ReceiveAsync(default))
        {
            _logger.LogInformation("Price {price}, Change {change}", ticker.trade_price, ticker.change);
        }
        IsBusy = false;
    }
}
