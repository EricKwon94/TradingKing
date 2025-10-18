using Application.Gateways.Hubs;
using Application.Services;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Gateways;

internal class RankingHub : IRankingApi
{
    private readonly IPreferences _preferences;
    private readonly Uri _uri;

    private readonly ConcurrentQueue<HubConnection> _connections = [];
    private HubConnection _connection = null!;

    public RankingHub(IPreferences preferences, Uri uri)
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

    public IAsyncEnumerable<Dictionary<string, double>> GetRank(long start, long stop = -1, CancellationToken ct = default)
    {
        return _connection.StreamAsync<Dictionary<string, double>>("GetRank", start, stop, ct);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connections.TryDequeue(out var connection))
        {
            await connection.DisposeAsync();
        }
    }
}
