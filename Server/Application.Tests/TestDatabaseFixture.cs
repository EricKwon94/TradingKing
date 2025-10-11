using Domain;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Application.Tests;

public class TestDatabaseFixture
{
    private const string DBNAME = "TradingKingTests";
    private const string WINDOWS = $"Server=(localdb)\\MSSQLLocalDB;Database={DBNAME};";
    private const string OTHERES = $"Server=localhost,1433;Database={DBNAME};User Id=sa;Password=Strongsanklfj2045!@$#%jsjS;TrustServerCertificate=True";

    private static readonly ConcurrentQueue<string> _ids = [];
    private static readonly Lock _lock = new();
    private static bool _databaseInitialized;

    public string IndependentId => _ids.TryDequeue(out var r) ? r : throw new Exception("큐없음");

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                var bogus = new Bogus.Randomizer();
                for (int i = 0; i < 100; i++)
                {
                    string str = bogus.String2(4, 10, "abcdefghijklmnopqrstuvwxyz1234567890가나다라마바사아자차카타파하");
                    _ids.Enqueue(str);
                }

                using var context = CreateContext();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var user = new User(IndependentId, "jkansklfndlk2048@");
                context.Users.Add(user);
                context.SaveChanges();

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
