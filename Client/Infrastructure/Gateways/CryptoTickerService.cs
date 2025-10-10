using Application.Gateways;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Gateways;

internal partial class CryptoTickerService : ICryptoTickerService
{
    private ClientWebSocket _currentSocket = null!;
    private readonly ConcurrentQueue<ClientWebSocket> _sockets = [];

    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        var socket = new ClientWebSocket();
        _sockets.Enqueue(socket);
        _currentSocket = socket;
        return socket.ConnectAsync(new Uri("wss://api.upbit.com/websocket/v1"), cancellationToken);
    }

    public ValueTask SendAsync(string[] codes, CancellationToken cancellationToken)
    {
        var a = new { ticket = Guid.NewGuid().ToString() };
        var b = new { type = "ticker", codes };
        var c = new { format = "DEFAULT" };
        object[] body = [a, b, c];

        string json = JsonSerializer.Serialize(body);
        ReadOnlyMemory<byte> buffer = Encoding.UTF8.GetBytes(json);

        return _currentSocket.SendAsync(buffer, WebSocketMessageType.Binary, true, cancellationToken);
    }

    public async IAsyncEnumerable<ICryptoTickerService.TickerRes> ReceiveAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (_currentSocket.State == WebSocketState.Open)
        {
            using var owner = MemoryPool<byte>.Shared.Rent(2048);
            Memory<byte> buffer = owner.Memory;

            ValueWebSocketReceiveResult result = await _currentSocket.ReceiveAsync(buffer, cancellationToken);
            ICryptoTickerService.TickerRes ticker = JsonSerializer.Deserialize<ICryptoTickerService.TickerRes>(buffer.Span[..result.Count])!;

            yield return ticker;

            await Task.Delay(100, cancellationToken);
        }
    }

    public void Dispose()
    {
        if (_sockets.TryDequeue(out var socket))
        {
            socket.Dispose();
        }
    }
}
