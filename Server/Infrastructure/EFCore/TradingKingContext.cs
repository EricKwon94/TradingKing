using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrastructure.EFCore;

internal class TradingKingContext : DbContext, IEntityTypeConfiguration<User>
{
    public DbSet<User> Users { get; set; }

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
        builder.HasKey(e => e.Seq);

        builder.HasIndex(e => e.Id).IsUnique();

        builder.Property(e => e.Id)
            .HasMaxLength(User.MAX_ID_LENGTH)
            .IsRequired()
            .IsUnicode(false);

        builder.Property(e => e.Password)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Jwt)
            .HasMaxLength(1000)
            .IsUnicode(false);

        builder.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("getutcdate()");
    }
}
