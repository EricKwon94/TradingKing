using Application.Gateways.Hubs;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModel.ViewModels;

public partial class ChatViewModel : BaseViewModel
{
    private readonly ILogger<ChatViewModel> _logger;
    private readonly IChatApi _chat;

    public ChatViewModel(ILogger<ChatViewModel> logger, IChatApi chat)
    {
        _logger = logger;
        _chat = chat;
    }

    public override async Task OnAppearingAsync(CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
            return;

        await _chat.StartAsync(ct);
        IDisposable disposable = _chat.ReceiveMessage((name, msg) =>
        {
            _logger.LogInformation("{name}: {msg}", name, msg);
            return Task.CompletedTask;
        });
    }

    public override async void OnDisappearing()
    {
        await _chat.DisposeAsync();
    }

    [RelayCommand]
    public Task InvokeAsync(CancellationToken ct)
    {
        return _chat.Echo("asd", "msg");
    }
}
