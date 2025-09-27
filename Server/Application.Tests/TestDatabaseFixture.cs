using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Application.Tests;

public class TestDatabaseFixture
{
    private const string WINDOWS = "Server=(localdb)\\MSSQLLocalDB;Database=TradingKingTests;";
    private const string OTHERES = "Server=localhost,1433;Database=TradingKingTests;User Id=sa;Password=Strongsanklfj2045!@$#%jsjS;TrustServerCertificate=True";

    private static readonly Lock _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using var context = CreateContext();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                _databaseInitialized = true;
            }
        }
    }

    internal TradingKingContext CreateContext()
    {
        string connectionString = OperatingSystem.IsWindows() ? WINDOWS : OTHERES;

        var builder = new DbContextOptionsBuilder<TradingKingContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);

        return new TradingKingContext(builder.Options);
    }
}
