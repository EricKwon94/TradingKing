using Application.Services;
using Microsoft.Maui.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

internal class NavigationService : INavigationService
{
    public Task GoToAsync(string uri, CancellationToken ct)
    {
        return Shell.Current.GoToAsync(uri);
    }
}
