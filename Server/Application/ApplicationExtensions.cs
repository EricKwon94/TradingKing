using Application.Orchestrations;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection builder)
    {
        return builder
            .AddTransient<AccountService>()
            .AddTransient<PurchaseService>()
            ;
    }
}
