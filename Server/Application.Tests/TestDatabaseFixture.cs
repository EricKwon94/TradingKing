using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Application.Tests;

public class TestDatabaseFixture
{
    private const string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TradingKingTests;";

    private static readonly Lock _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
                _databaseInitialized = true;
            }
        }
    }

    internal TradingKingContext CreateContext()
    {
        var builder = new DbContextOptionsBuilder<TradingKingContext>()
            .UseSqlServer(ConnectionString)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);

        return new TradingKingContext(builder.Options);
    }
}
