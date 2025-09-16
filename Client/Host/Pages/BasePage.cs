using Microsoft.Maui.Controls;
using ViewModel.ViewModels;

namespace Host.Pages;

public partial class BasePage : ContentPage
{
    public BasePage(BaseViewModel vm)
    {
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is BaseViewModel vm)
        {
            vm.OnAppearing();
        }
    }
}
