using Microsoft.EntityFrameworkCore;

namespace Shared;

public class TradingKingContext : DbContext
{
    public DbSet<OrderModel> Orders { get; private set; }
    public DbSet<SeasonModel> Seasons { get; private set; }
    public DbSet<UserModel> Users { get; private set; }

    public TradingKingContext(DbContextOptions<TradingKingContext> options)
        : base(options)
    {
    }
}
