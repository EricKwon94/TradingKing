using Domain.Persistences;
using Infrastructure.EFCore;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection builder, bool isDevelopment, string sqlserverCs)
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
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IOrderRepo, OrderRepo>()
            .AddTransient<ISeasonRepo, SeasonRepo>()
            .AddTransient<IRankRepo, RankRepo>()
            .AddTransient<Domain.Persistences.ITransaction, Transaction>()
        ;
    }
}
