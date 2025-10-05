using Microsoft.Maui.Controls;
using System;
using View.Pages;

namespace View;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("login/register", typeof(RegisterPage));
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        if (Current?.CurrentPage.BindingContext is IDisposable disposable
            && (args.Source == ShellNavigationSource.Pop
            || args.Source == ShellNavigationSource.PopToRoot
            || args.Source == ShellNavigationSource.Remove
            || args.Source == ShellNavigationSource.ShellItemChanged))
        {
            disposable.Dispose();
        }
    }
}
