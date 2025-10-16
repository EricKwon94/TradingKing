using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Infrastructure.EFCore;

internal class TradingKingContext : DbContext, IEntityTypeConfiguration<User>, IEntityTypeConfiguration<Order>
{
    public DbSet<User> Users { get; private set; }
    public DbSet<Order> Orders { get; private set; }

    public TradingKingContext(DbContextOptions<TradingKingContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration<User>(this);
        modelBuilder.ApplyConfiguration<Order>(this);
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasMaxLength(User.MAX_ID_LENGTH)
            .IsRequired()
            .IsUnicode(true);

        builder.Property(e => e.Password)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Jwt)
            .HasMaxLength(1000)
            .IsUnicode(false);

        builder.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("getutcdate()");
    }

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasDefaultValueSql("newid()");

        builder.Property(e => e.Code)
            .HasMaxLength(20)
            .IsUnicode(false);
        builder.Property(e => e.Quantity);
        builder.Property(e => e.Price);

        builder.HasOne(oder => oder.User)
            .WithMany(user => user.Orders)
            .HasForeignKey(oder => oder.UserId);

        builder.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("getutcdate()");
    }
}
