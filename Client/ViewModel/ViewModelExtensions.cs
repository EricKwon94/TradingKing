using Microsoft.Extensions.DependencyInjection;
using ViewModel.ViewModels;
using ViewModel.ViewModels.Trade;

namespace ViewModel;

public static class ViewModelExtensions
{
    public static IServiceCollection AddViewModel(this IServiceCollection builder)
    {
        return builder.AddTransient<LoginViewModel>()
            .AddTransient<RegisterViewModel>()
            .AddTransient<TradeViewModel>()
            .AddTransient<RankingViewModel>()
            .AddTransient<AssetViewModel>()
            .AddTransient<SeasonViewModel>()
            ;
    }
}
