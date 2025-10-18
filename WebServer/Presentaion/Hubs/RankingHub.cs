using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ranks = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, double>>;

namespace Presentaion.Hubs;

[Authorize]
public class RankingHub : Hub
{
    private readonly ILogger<RankingHub> _logger;
    private readonly ICacheService _cache;

    public RankingHub(ILogger<RankingHub> logger, ICacheService cache)
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
            var ranks = await _cache.GetRankAsync(start, stop, ct);
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
