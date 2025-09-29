using Microsoft.Maui.Controls;
using ViewModel.ViewModels;

namespace View.Pages;

public partial class BasePage : ContentPage
{
    public BasePage(BaseViewModel vm)
    {
        BindingContext = vm;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is BaseViewModel vm)
        {
            vm.Initialize();
        }
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
