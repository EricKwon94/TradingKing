using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewModel.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy = false;

    public virtual void Initialize()
    {

    }

    public virtual void OnAppearing()
    {

    }
}
