using Microsoft.Maui.Controls;
using System;

namespace Infrastructure.Services;

internal class Dispatcher : Application.Services.IDispatcher
{
    public void Invoke(Action action)
    {
        Shell.Current.Dispatcher.Dispatch(action);
    }
}
