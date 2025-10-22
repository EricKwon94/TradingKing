using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface ISeasonRepo
{
    Task<List<Season>> GetSeasonsAsync(CancellationToken ct);
    Task<int> GetLastSeasonIdAsync(CancellationToken ct);
}
