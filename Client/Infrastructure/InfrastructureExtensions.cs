using Application.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection builder)
    {
        return builder.AddTransient<IPreferences, Preferences>()
            .AddTransient<IAlertService, AlertService>();
    }
}
