using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IPurchaseRepo
{
    Task<List<Purchase>> GetPurchasesAsync(int userSeq, CancellationToken ct);
    ValueTask AddPurchaseAsync(Purchase purchase, CancellationToken ct);
}
