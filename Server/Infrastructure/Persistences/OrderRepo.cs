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
    private readonly DbSet<Order> _orders;

    public OrderRepo(TradingKingContext context)
    {
        _orders = context.Orders;
    }

    public Task<List<Order>> GetAllAsync(int userSeq, CancellationToken ct)
    {
        return _orders.AsNoTracking()
            .Where(e => e.UserSeq == userSeq)
            .ToListAsync(ct);
    }

    public async ValueTask AddAsync(Order purchase, CancellationToken ct)
    {
        await _orders.AddAsync(purchase, ct);
    }
}
