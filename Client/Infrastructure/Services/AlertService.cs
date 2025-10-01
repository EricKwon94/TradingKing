using Application.Services;
using Microsoft.Maui.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services;

internal class AlertService : IAlertService
{
    public Task DisplayAlertAsync(string title, string message, string cancel, CancellationToken ct)
    {
        return Shell.Current.CurrentPage.DisplayAlert(title, message, cancel);
    }
}
