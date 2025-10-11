using Domain;
using Domain.Persistences;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistences;

internal class PurchaseRepo : IPurchaseRepo
{
    private readonly DbSet<Purchase> _purchases;

    public PurchaseRepo(TradingKingContext context)
    {
        _purchases = context.Purchases;
    }

    public async Task<List<Purchase>> GetPurchasesAsync(int userSeq, CancellationToken ct)
    {
        return await _purchases.AsNoTracking()
            .Where(e => e.UserSeq == userSeq)
            .ToListAsync(ct);
    }

    public async ValueTask AddPurchaseAsync(Purchase purchase, CancellationToken ct)
    {
        await _purchases.AddAsync(purchase, ct);
    }
}
