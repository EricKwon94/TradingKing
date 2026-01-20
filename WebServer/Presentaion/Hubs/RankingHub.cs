using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ranks = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, double>>;

namespace Presentaion.Hubs;

[Authorize]
public class RankingHub : Hub
{
    private readonly ILogger<RankingHub> _logger;
    private readonly ConcurrentDictionary<string, double> _cache;

    public RankingHub(ILogger<RankingHub> logger, [FromKeyedServices(Constant.CACHE_KEY)] ConcurrentDictionary<string, double> cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("{seq} 입장", Context.UserIdentifier);
        return base.OnConnectedAsync();
    }

    public async IAsyncEnumerable<Ranks> GetRank(long start, long stop = -1, [EnumeratorCancellation] CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            var ranks = _cache.OrderByDescending(e => e.Value).ToDictionary();
            yield return ranks;
            await Task.Delay(1000, ct);
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception == null)
            _logger.LogInformation("{seq} 퇴장", Context.UserIdentifier);
        else
            _logger.LogError(exception, "{seq} 퇴장", Context.UserIdentifier);
        return base.OnDisconnectedAsync(exception);
    }
}
