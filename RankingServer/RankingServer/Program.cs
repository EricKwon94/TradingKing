using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Refit;
using Shared;

namespace RankingServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddDbContext<TradingKingContext>(opt =>
        {
            string connectionString = builder.Configuration.GetConnectionString("TradingKing")!;
#if DEBUG
            opt.UseSqlServer(connectionString);
#else
                opt.UseAzureSql(connectionString);
#endif
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        string serviceBusCs = builder.Configuration["ServiceBus"]!;
        string orderQueueName = builder.Configuration["OrderQueueName"]!;
        string rankQueueName = builder.Configuration["RankQueueName"]!;

        builder.Services.AddSingleton(p => new ServiceBusClient(serviceBusCs));
        builder.Services.AddKeyedSingleton("order", (p, key) =>
        {
            var client = p.GetRequiredService<ServiceBusClient>();
            return client.CreateReceiver(orderQueueName);
        });
        builder.Services.AddKeyedSingleton("rank", (p, key) =>
        {
            var client = p.GetRequiredService<ServiceBusClient>();
            return client.CreateReceiver(rankQueueName);
        });

        var upbitApi = RestService.For<IExchangeApi>("https://api.upbit.com");
        builder.Services.AddSingleton(upbitApi);

        var host = builder.Build();
        host.Run();
    }
}