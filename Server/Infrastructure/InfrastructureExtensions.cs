using Domain.Persistences;
using Infrastructure.EFCore;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection builder, bool isDevelopment, string connectionString)
    {
        return builder
            .AddDbContext<TradingKingContext>(opt =>
            {
                /*opt.UseInMemoryDatabase("TradingKing");
                return;*/
                if (isDevelopment)
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }
#if DEBUG
                opt.UseSqlServer(connectionString);
#else
                opt.UseAzureSql(connectionString);
#endif
            })
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<ITransaction, Transaction>()
        ;
    }
}
