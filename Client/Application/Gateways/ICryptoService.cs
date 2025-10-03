using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Gateways;

public interface ICryptoService
{
    [Get("/v1/market/all")]
    Task<IEnumerable<CryptoAsset>> GetAssetsAsync(CancellationToken ct);

    public record CryptoAsset(string market, string korean_name, string english_name);
}

public interface ICryptoTickerService : IDisposable
{
    Task ConnectAsync(CancellationToken cancellationToken);
    ValueTask SendAsync(string[] codes, CancellationToken cancellationToken);
    IAsyncEnumerable<Ticker> ReceiveAsync(CancellationToken cancellationToken);

    public record Ticker(string type, string code, double trade_price, string change);
}
