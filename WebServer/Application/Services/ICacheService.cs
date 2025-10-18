using System.Collections.Frozen;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public interface ICacheService
{
    Task<FrozenDictionary<string, double>> GetRankAsync(long start, long stop = -1, CancellationToken ct = default);
}
