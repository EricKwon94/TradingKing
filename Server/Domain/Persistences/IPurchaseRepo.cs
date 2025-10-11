using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IPurchaseRepo
{
    Task<List<Purchase>> GetAllAsync(int userSeq, CancellationToken ct);
    ValueTask AddAsync(Purchase purchase, CancellationToken ct);
}
