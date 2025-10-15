using Application.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Gateways;

internal class ChatHub : Application.Gateways.Hubs.IChatApi
{
    private readonly IPreferences _preferences;
    private readonly Uri _uri;

    private readonly ConcurrentQueue<HubConnection> _connections = [];
    private HubConnection _connection = null!;

    public ChatHub(IPreferences preferences, Uri uri)
    {
        _preferences = preferences;
        _uri = uri;
    }

    public Task StartAsync(CancellationToken ct)
    {
        string jwt = _preferences.Get("jwt", "");
        _connection = new HubConnectionBuilder()
                .WithUrl(_uri, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(jwt)!;
                })
                .Build();
        _connections.Enqueue(_connection);
        return _connection.StartAsync(ct);
    }

    public IDisposable ReceiveMessage(Func<string, string, Task> handler)
    {
        return _connection.On(nameof(Application.Gateways.IChatClient.ReceiveMessage), handler);
    }

    public Task BroadcastMessage(string name, string message)
    {
        return _connection.InvokeAsync(nameof(BroadcastMessage), name, message);
    }

    public Task Echo(string name, string message)
    {
        return _connection.InvokeAsync(nameof(Echo), name, message);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connections.TryDequeue(out var connection))
        {
            await connection.DisposeAsync();
        }
    }
}
