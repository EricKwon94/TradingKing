using Infrastructure.EFCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;

namespace IntegrationTests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private const string DBNAME = "TradingKingTests2";
    private const string WINDOWS = $"Server=(localdb)\\MSSQLLocalDB;Database={DBNAME};";
    private const string OTHERES = $"Server=localhost,1433;Database={DBNAME};User Id=sa;Password=Strongsanklfj2045!@$#%jsjS;TrustServerCertificate=True";

    private static readonly ConcurrentQueue<string> _ids = [];
    private static readonly string ConnectionString = OperatingSystem.IsWindows() ? WINDOWS : OTHERES;

    public string IndependentId => _ids.TryDequeue(out var r) ? r : throw new System.Exception("큐없음");

    static CustomWebApplicationFactory()
    {
        var bogus = new Bogus.Randomizer();
        for (int i = 0; i < 100; i++)
        {
            string str = bogus.String2(4, 10, "abcdefghijklmnopqrstuvwxyz1234567890가나다라마바사아자차카타파하");
            _ids.Enqueue(str);
        }

        var builder = new DbContextOptionsBuilder<TradingKingContext>()
            .UseSqlServer(ConnectionString);

        using var context = new TradingKingContext(builder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ISS_KEY", "x08z5dZcVX4QDJa3!QumT8?5y1Ezzp2bXjzHeDgzfR");
        builder.ConfigureTestServices(services =>
        {
            var dbContextDescriptor = services.Single(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<TradingKingContext>));
            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            services.AddDbContext<TradingKingContext>((provider, opt) =>
            {
                opt.UseSqlServer(ConnectionString);
            });
        });
    }
}
