using Application.Api;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Application.Api.IAccountService;

namespace ViewModel.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAccountService _accountService;
    private readonly IAlertService _alert;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _userId = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _userNickname = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string _userPassword = string.Empty;

    [ObservableProperty]
    private int? _minIdLength;
    [ObservableProperty]
    private int? _maxIdLength;
    [ObservableProperty]
    private int? _minNicknameLength;
    [ObservableProperty]
    private int? _maxNicknameLength;
    [ObservableProperty]
    private int? _minPasswordLength;

    public RegisterViewModel(IAccountService accountService, IAlertService alert, INavigationService navigationService)
    {
        _accountService = accountService;
        _alert = alert;
        _navigationService = navigationService;
    }

    [RelayCommand(CanExecute = nameof(CanRegister))]
    public async Task RegisterAsync(CancellationToken ct)
    {
        var body = new RegisterReq(UserId, UserNickname, UserPassword);
        try
        {
            await _accountService.RegisterAsync(body, ct);
        }
        catch (ApiException e) when (int.TryParse(e.Content, out int errorCode))
        {
            await ShowErrorAsync(e.StatusCode, errorCode, default);
            return;
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", ct);
            return;
        }
        await _alert.DisplayAlertAsync("회원가입", "가입성공", "ok", default);
        await _navigationService.GoToAsync("..", ct);
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
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", default);
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

    private bool CanRegister()
    {
        if (UserId.Length >= MinIdLength && UserId.Length <= MaxIdLength &&
            UserNickname.Length >= MinNicknameLength && UserNickname.Length <= MaxNicknameLength &&
            UserPassword.Length >= MinPasswordLength)
            return true;
        return false;
    }

    private Task ShowErrorAsync(HttpStatusCode statusCode, int errorCode, CancellationToken ct)
    {
        if (statusCode == HttpStatusCode.BadRequest)
        {
            if (errorCode == -1)
                return _alert.DisplayAlertAsync("Error", "ID는 영문, 숫자", "ok", ct);
            else if (errorCode == -2)
                return _alert.DisplayAlertAsync("Error", "닉네임은 영어, 완성된 한글, 숫자", "ok", ct);
            else if (errorCode == -3)
                return _alert.DisplayAlertAsync("Error", "패스워드 이상함", "ok", ct);
        }
        else if (statusCode == HttpStatusCode.Conflict)
        {
            if (errorCode == -1)
                return _alert.DisplayAlertAsync("Error", "ID 중복", "ok", ct);
            else if (errorCode == -2)
                return _alert.DisplayAlertAsync("Error", "닉네임 중복", "ok", ct);
            else if (errorCode == -3)
                return _alert.DisplayAlertAsync("Error", "계정 중복", "ok", ct);
        }
        return Task.CompletedTask;
    }
}
