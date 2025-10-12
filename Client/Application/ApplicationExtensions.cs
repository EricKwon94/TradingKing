using Application.Gateways;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Refit;
using System;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection builder, Uri address)
    {
        builder.AddTransient<AuthHeaderHandler>();

        builder.AddRefitClient<IExchangeApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.upbit.com"))
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, count => TimeSpan.FromSeconds(2 * count)))
            ;

        builder.AddRefitClient<IAccountApi>()
            .ConfigureHttpClient(c => c.BaseAddress = address)
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, count => TimeSpan.FromSeconds(2 * count)))
            .AddHttpMessageHandler<AuthHeaderHandler>()
            ;

        builder.AddRefitClient<IOrderApi>()
            .ConfigureHttpClient(c => c.BaseAddress = address)
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, count => TimeSpan.FromSeconds(2 * count)))
            .AddHttpMessageHandler<AuthHeaderHandler>()
            ;
        return builder;
    }
}
