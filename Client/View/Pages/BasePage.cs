using Microsoft.Maui.Controls;
using System.Collections.Generic;
using ViewModel.ViewModels;

namespace View.Pages;

public partial class BasePage : ContentPage, IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (BindingContext is ViewModel.Contracts.IQueryAttributable attributable)
        {
            attributable.ApplyQueryAttributes(query);
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is BaseViewModel vm)
        {
            vm.Initialize();
        }
    }
}
