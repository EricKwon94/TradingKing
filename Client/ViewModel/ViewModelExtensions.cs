using Microsoft.Extensions.DependencyInjection;

namespace ViewModel;

public static class ViewModelExtensions
{
    public static IServiceCollection AddViewModel(this IServiceCollection builder)
    {
        return builder.AddTransient<ViewModels.LoginViewModel>();
    }
}
