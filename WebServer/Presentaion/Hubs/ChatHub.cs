using Application.Gateways;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Presentaion.Hubs;

[Authorize]
public class ChatHub : Hub<IChatClient>, IChatHub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("{seq} 입장", Context.UserIdentifier);
        return base.OnConnectedAsync();
    }

    public Task BroadcastMessage(string name, string message)
    {
        return Clients.All.ReceiveMessage(name, message);
    }

    public Task Echo(string name, string message)
    {
        return Clients.Caller.ReceiveMessage($"echo: {name}", message);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception == null)
            _logger.LogInformation("{seq} 퇴장", Context.UserIdentifier);
        else
            _logger.LogError(exception, "{seq} 퇴장", Context.UserIdentifier);
        return base.OnDisconnectedAsync(exception);
    }
}
