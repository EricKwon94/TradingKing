using Domain;
using Domain.Persistences;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistences;

internal class OrderRepo : IOrderRepo
{
    private readonly DbSet<Order> _orderes;

    public OrderRepo(TradingKingContext context)
    {
        _orderes = context.Orderes;
    }

    public Task<List<Order>> GetAllAsync(int userSeq, CancellationToken ct)
    {
        return _orderes.AsNoTracking()
            .Where(e => e.UserSeq == userSeq)
            .ToListAsync(ct);
    }

    public async ValueTask AddAsync(Order purchase, CancellationToken ct)
    {
        await _orderes.AddAsync(purchase, ct);
    }
}
