using Application.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

internal class AlertService : IAlertService
{
    public Task DisplayAlertAsync(string title, string message, string cancel, CancellationToken ct)
    {
        var windows = Microsoft.Maui.Controls.Application.Current!.Windows;
        int lastIndex = windows.Count - 1;
        var page = windows[lastIndex].Page;

        return page!.DisplayAlert(title, message, cancel);
    }
}
