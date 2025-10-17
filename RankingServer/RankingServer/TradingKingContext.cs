using Microsoft.EntityFrameworkCore;
using Shared;

namespace RankingServer;

internal class TradingKingContext : DbContext
{
    public DbSet<OrderModel> Orders { get; private set; }

    public TradingKingContext(DbContextOptions<TradingKingContext> options)
        : base(options)
    {
    }
}
