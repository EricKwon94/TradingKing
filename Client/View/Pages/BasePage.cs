using Microsoft.Maui.Controls;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ViewModel.ViewModels;

namespace View.Pages;

public partial class BasePage : ContentPage, IQueryAttributable
{
    private readonly ConcurrentQueue<CancellationTokenSource> _cts = [];
    private Task? _loadTask;

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
            vm.IsBusy = true;

            var cts = new CancellationTokenSource();
            _cts.Enqueue(cts);
            _loadTask = vm.LoadAsync(cts.Token);
        }
    }

    protected override async void OnAppearing()
    {
        if (BindingContext is BaseViewModel vm)
        {
            vm.IsBusy = true;

            var cts = new CancellationTokenSource();
            _cts.Enqueue(cts);
            if (_loadTask != null)
                await _loadTask;
            await vm.OnAppearingAsync(cts.Token);

            vm.IsBusy = false;
        }
    }

    protected override void OnDisappearing()
    {
        if (BindingContext is BaseViewModel vm)
        {
            while (_cts.TryDequeue(out var cts))
            {
                cts.Cancel();
            }
            vm.OnDisappearing();
        }
    }
}
