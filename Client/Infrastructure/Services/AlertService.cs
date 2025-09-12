using Application.Services;
using System.Threading.Tasks;

namespace Infrastructure.Services;

internal class AlertService : IAlertService
{
    public Task DisplayAlert(string title, string message, string cancel)
    {
        var windows = Microsoft.Maui.Controls.Application.Current!.Windows;
        int lastIndex = windows.Count - 1;
        var page = windows[lastIndex].Page;

        return page!.DisplayAlert(title, message, cancel);
    }
}
