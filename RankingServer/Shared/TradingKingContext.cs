using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared;

public class TradingKingContext : DbContext, IEntityTypeConfiguration<RankModel>
{
    public DbSet<OrderModel> Orders { get; private set; }
    public DbSet<SeasonModel> Seasons { get; private set; }
    public DbSet<UserModel> Users { get; private set; }
    public DbSet<RankModel> Ranks { get; private set; }

    public TradingKingContext(DbContextOptions<TradingKingContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration<RankModel>(this);
    }

    public void Configure(EntityTypeBuilder<RankModel> builder)
    {
        builder.HasKey(e => new { e.SeasonId, e.UserId });
    }
}
