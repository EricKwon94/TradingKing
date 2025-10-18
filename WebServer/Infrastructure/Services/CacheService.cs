using Application.Services;
using StackExchange.Redis;
using System.Collections.Frozen;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

internal class CacheService : ICacheService
{
    private const string KEY = "ranking";

    private readonly IDatabase _database;

    public CacheService(ConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    public async Task<FrozenDictionary<string, double>> GetRankAsync(long start, long stop = -1, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        SortedSetEntry[] entries = await _database.SortedSetRangeByRankWithScoresAsync(KEY, start, stop, Order.Descending);
        return entries.ToFrozenDictionary(e => e.Element.ToString(), e => e.Score);
    }
}
