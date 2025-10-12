using Application.Gateways;
using Application.Orchestrations;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection builder)
    {
        builder.AddRefitClient<IExchangeApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.upbit.com"))
            ;

        return builder
            .AddTransient<AccountService>()
            .AddTransient<OrderService>()
            ;
    }
}
