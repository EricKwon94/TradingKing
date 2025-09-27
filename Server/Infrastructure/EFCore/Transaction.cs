using Domain.Persistences;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.EFCore;

internal class Transaction : ITransaction
{
    private readonly TradingKingContext _dbContext;

    public Transaction(TradingKingContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _dbContext.SaveChangesAsync(ct);
    }
}
