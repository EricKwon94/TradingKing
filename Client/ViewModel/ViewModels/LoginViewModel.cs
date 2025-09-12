using Application.Api;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAccountService _accountService;
    private readonly IPreferences _preferences;
    private readonly IAlertService _alert;

    [ObservableProperty]
    private int count;

    public LoginViewModel(IAccountService accountService, IPreferences preferences, IAlertService alert)
    {
        _accountService = accountService;
        _preferences = preferences;
        _alert = alert;
    }

    [RelayCommand]
    public async Task Click(CancellationToken ct)
    {
        Count++;
        string token = _preferences.Get("token", "");
        if (string.IsNullOrEmpty(token))
        {
            token = await _accountService.Login();
            _preferences.Set("token", token);
        }
    }

    [RelayCommand]
    public async Task User(CancellationToken ct)
    {
        _preferences.Clear();
        try
        {
            string s = await _accountService.User();
        }
        catch (Exception e)
        {
            await _alert.DisplayAlert("Error", e.Message, "ok");
        }
    }
}
