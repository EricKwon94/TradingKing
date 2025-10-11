using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    public virtual Task LoadAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnAppearingAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public virtual void OnDisappearing()
    {

    }
}
