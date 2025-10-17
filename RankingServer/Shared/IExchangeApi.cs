using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared;

public interface IExchangeApi
{
    [Get("/v1/ticker")]
    Task<IEnumerable<TickerRes>> GetTickerAsync(IEnumerable<string> markets, CancellationToken ct);

    public record TickerRes(string market, double trade_price);
}
