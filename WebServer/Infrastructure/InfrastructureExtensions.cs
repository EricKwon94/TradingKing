using Application.Services;
using Domain.Persistences;
using Infrastructure.EFCore;
using Infrastructure.Persistences;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection builder, bool isDevelopment, string sqlserverCs, string redisCs)
    {
        return builder
            .AddDbContext<TradingKingContext>(opt =>
            {
                if (isDevelopment)
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }
#if DEBUG
                opt.UseSqlServer(sqlserverCs);
#else
                opt.UseAzureSql(sqlserverCs);
#endif
            })
            .AddSingleton(p => ConnectionMultiplexer.Connect(redisCs))
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IOrderRepo, OrderRepo>()
            .AddTransient<ISeasonRepo, SeasonRepo>()
            .AddTransient<IRankRepo, RankRepo>()
            .AddTransient<Domain.Persistences.ITransaction, Transaction>()
            .AddTransient<ICacheService, CacheService>()
        ;
    }
}
