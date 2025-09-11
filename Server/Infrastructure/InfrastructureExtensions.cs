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
            ;
    }
}
