using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrastructure;

internal class TradingKingContext : DbContext, IEntityTypeConfiguration<User>
{
    public DbSet<User> Accounts { get; set; }

    public TradingKingContext(DbContextOptions<TradingKingContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(this);
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.No);
        builder.HasIndex(e => e.Id).IsUnique();

        builder.Property(e => e.Id)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("getutcdate()");
    }
}
