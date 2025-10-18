using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Gateways.Hubs;

public interface IRankingApi : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct);
    IAsyncEnumerable<Dictionary<string, double>> GetRank(long start, long stop = -1, CancellationToken ct = default);
}
