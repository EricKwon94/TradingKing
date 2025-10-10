using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Gateways;

public interface ICryptoService
{
    [Get("/v1/market/all")]
    Task<IEnumerable<MarketRes>> GetMarketsAsync(CancellationToken ct);

    public record MarketRes(string market, string korean_name, string english_name);
}

public interface ICryptoTickerService : IDisposable
{
    Task ConnectAsync(CancellationToken cancellationToken);
    ValueTask SendAsync(string[] codes, CancellationToken cancellationToken);
    IAsyncEnumerable<TickerRes> ReceiveAsync(CancellationToken cancellationToken);

    public record TickerRes(string type, string code, double trade_price, string change);
}
