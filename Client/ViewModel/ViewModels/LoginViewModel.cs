using Application.Api;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Application.Api.IAccountService;

namespace ViewModel.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAccountService _accountService;
    private readonly IPreferences _preferences;
    private readonly IAlertService _alert;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private int _minIdLength;
    [ObservableProperty]
    private int _maxIdLength;
    [ObservableProperty]
    private int _minNicknameLength;
    [ObservableProperty]
    private int _maxNicknameLength;
    [ObservableProperty]
    private int _minPasswordLength;

    public LoginViewModel(
        IAccountService accountService, IPreferences preferences,
        IAlertService alert, INavigationService navigationService)
    {
        _accountService = accountService;
        _preferences = preferences;
        _alert = alert;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task LoginAsync(CancellationToken ct)
    {

    }

    [RelayCommand]
    public Task GoToRegisterAsync(CancellationToken ct)
    {
        return _navigationService.GoToAsync("login/register", ct);
    }

    public override async void OnAppearing()
    {
        IsBusy = true;

        FormRes? res = null;
        try
        {
            res = await _accountService.GetFormAsync(default);
        }
        catch (Exception e)
        {
            await _alert.DisplayAlert("Error", e.Message, "ok");
        }

        if (res != null)
        {
            MinIdLength = res.MinIdLength;
            MaxIdLength = res.MaxIdLength;
            MinNicknameLength = res.MinNicknameLength;
            MaxNicknameLength = res.MaxNicknameLength;
            MinPasswordLength = res.MinPasswordLength;
        }

        IsBusy = false;
    }
}
