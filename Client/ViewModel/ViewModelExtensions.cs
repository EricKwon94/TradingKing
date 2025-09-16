using Microsoft.Extensions.DependencyInjection;
using ViewModel.ViewModels;

namespace ViewModel;

public static class ViewModelExtensions
{
    public static IServiceCollection AddViewModel(this IServiceCollection builder)
    {
        return builder.AddTransient<LoginViewModel>()
            .AddTransient<RegisterViewModel>()
            ;
    }
}
