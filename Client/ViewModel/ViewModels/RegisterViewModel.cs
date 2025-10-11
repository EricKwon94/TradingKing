using Application.Gateways;
using Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Refit;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Application.Gateways.IAccountService;

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
    private string _userPassword = string.Empty;

    [ObservableProperty]
    private int? _minIdLength;
    [ObservableProperty]
    private int? _maxIdLength;
    [ObservableProperty]
    private int? _minPasswordLength;

    public RegisterViewModel(IAccountService accountService, IAlertService alert, INavigationService navigationService)
    {
        _accountService = accountService;
        _alert = alert;
        _navigationService = navigationService;
    }

    public override async Task LoadAsync(CancellationToken ct)
    {
        FormRes? res = null;
        try
        {
            res = await _accountService.GetFormAsync(ct);
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", ct);
        }

        if (res != null)
        {
            MinIdLength = res.MinIdLength;
            MaxIdLength = res.MaxIdLength;
            MinPasswordLength = res.MinPasswordLength;
        }
    }

    [RelayCommand(CanExecute = nameof(CanRegister))]
    public async Task RegisterAsync(CancellationToken ct)
    {
        var body = new RegisterReq(UserId, UserPassword);
        bool succ = false;
        try
        {
            await _accountService.RegisterAsync(body, ct);
            succ = true;
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.Conflict)
        {
            await _alert.DisplayAlertAsync("Error", "ID 중복", "ok", ct);
        }
        catch (ApiException e) when (e.StatusCode == HttpStatusCode.BadRequest && int.TryParse(e.Content, out int errorCode))
        {
            if (errorCode == -1)
                await _alert.DisplayAlertAsync("Error", "ID는 영문, 숫자, 완성된 한글", "ok", ct);
            else if (errorCode == -2)
                await _alert.DisplayAlertAsync("Error", "패스워드 이상함", "ok", ct);
        }
        catch (Exception e)
        {
            await _alert.DisplayAlertAsync("Error", e.Message, "ok", ct);
        }

        if (succ)
        {
            await _alert.DisplayAlertAsync("회원가입", "가입성공", "ok", ct);
            await _navigationService.GoToAsync("..", ct);
        }
    }

    private bool CanRegister()
    {
        if (UserId.Length >= MinIdLength && UserId.Length <= MaxIdLength &&
            UserPassword.Length >= MinPasswordLength)
            return true;
        return false;
    }
}
