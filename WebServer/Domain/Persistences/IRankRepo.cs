using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IRankRepo
{
    Task<List<Rank>> GetRanksAsync(int seasonId, CancellationToken ct);
}
