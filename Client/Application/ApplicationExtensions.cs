using Application.Api;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Refit;
using System;

namespace Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection builder)
    {
        builder.AddTransient<AuthHeaderHandler>();

        builder.AddRefitClient<IAccountService>()
            .ConfigureHttpClient(c => c.BaseAddress = Env.serverAddress)
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(4, count => TimeSpan.FromSeconds(2 * count)))
            .AddHttpMessageHandler<AuthHeaderHandler>()
            ;
        return builder;
    }
}
