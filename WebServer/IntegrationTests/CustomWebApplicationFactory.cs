using Application.Orchestrations;
using Domain;
using Infrastructure.EFCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        using var context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.Database.ExecuteSqlRaw("Insert into Seasons DEFAULT VALUES", []);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ISS_KEY", "x08z5dZcVX4QDJa3!QumT8?5y1Ezzp2bXjzHeDgzfR");
        builder.UseEnvironment("Test");
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

    public async Task<User> RegisterAsync(HttpClient client, string id, string pwd, bool autoLogin)
    {
        var content = new AccountService.RegisterReq(id, pwd).ToContent();
        var res = await client.PostAsync("/account/register", content);
        res.EnsureSuccessStatusCode();

        using var context = CreateContext();

        if (autoLogin)
        {
            var content2 = new AccountService.LoginReq(id, pwd).ToContent();
            HttpResponseMessage res2 = await client.PostAsync("/account/login", content2);
            res2.EnsureSuccessStatusCode();
            string jwt = await res2.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        }

        return await context.Users.AsNoTracking().SingleAsync(e => e.Id == id);
    }

    private static TradingKingContext CreateContext()
    {
        string connectionString = OperatingSystem.IsWindows() ? WINDOWS : OTHERES;

        var builder = new DbContextOptionsBuilder<TradingKingContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, LogLevel.Information);

        return new TradingKingContext(builder.Options);
    }
}
