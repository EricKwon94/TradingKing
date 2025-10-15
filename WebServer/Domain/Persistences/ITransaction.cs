using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface ITransaction
{
    Task SaveChangesAsync(CancellationToken ct);
}
