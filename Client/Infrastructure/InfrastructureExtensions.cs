using Application.Gateways;
using Application.Gateways.Hubs;
using Application.Services;
using Infrastructure.Gateways;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection builder, Uri uri)
    {
        return builder.AddTransient<IPreferences, Preferences>()
            .AddTransient<IAlertService, AlertService>()
            .AddTransient<INavigationService, NavigationService>()
            .AddTransient<IExchangeTickerApi, UpbitTickerApi>()
            .AddTransient<IDispatcher, Dispatcher>()
            .AddTransient<IChatApi, ChatHub>(provider =>
            {
                IPreferences preferences = provider.GetRequiredService<IPreferences>();
                return new ChatHub(preferences, new Uri(uri, "/chat"));
            })
            .AddTransient<IRankingApi, RankingHub>(provider =>
            {
                IPreferences preferences = provider.GetRequiredService<IPreferences>();
                return new RankingHub(preferences, new Uri(uri, "/ranking"));
            })
            ;
    }
}
