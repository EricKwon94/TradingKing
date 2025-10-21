using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface ISeasonRepo
{
    Task<int> GetLastSeasonIdAsync(CancellationToken ct);
}
