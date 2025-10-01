using Application.Api;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
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
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _userId = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _userPassword = string.Empty;

    [ObservableProperty]
    private int? _minIdLength;
    [ObservableProperty]
    private int? _maxIdLength;
    [ObservableProperty]
    private int? _minPasswordLength;

    public LoginViewModel(
        IAccountService accountService, IPreferences preferences,
        IAlertService alert, INavigationService navigationService)
    {
        _accountService = accountService;
        _preferences = preferences;
        _alert = alert;
        _navigationService = navigationService;
    }

    [RelayCommand(CanExecute = nameof(CanLogin))]
    public async Task LoginAsync(CancellationToken ct)
    {
        IsBusy = true;

        var body = new LoginReq(UserId, UserPassword);
        string? jwt = null;
        try
        {
            jwt = await _accountService.LoginAsync(body, ct);
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            await _alert.DisplayAlertAsync("Error", "계정을 찾을 수 없어", "ok", ct);
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", ct);
        }

        if (!string.IsNullOrEmpty(jwt))
        {
            _preferences.Set("jwt", jwt);
            var parameters = new Dictionary<string, object>
            {
                ["Id"] = UserId,
            };
            await _navigationService.GoToAsync("//main", parameters, ct);
        }

        IsBusy = false;
    }

    [RelayCommand]
    public Task GoToRegisterAsync(CancellationToken ct)
    {
        return _navigationService.GoToAsync("login/register", ct);
    }

    public override async void Initialize()
    {
        IsBusy = true;

        FormRes? res = null;
        try
        {
            res = await _accountService.GetFormAsync(default);
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", default);
        }

        if (res != null)
        {
            MinIdLength = res.MinIdLength;
            MaxIdLength = res.MaxIdLength;
            MinPasswordLength = res.MinPasswordLength;
        }

        IsBusy = false;
    }

    private bool CanLogin()
    {
        if (UserId.Length >= MinIdLength && UserId.Length <= MaxIdLength &&
            UserPassword.Length >= MinPasswordLength)
            return true;
        return false;
    }
}
