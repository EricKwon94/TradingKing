using Application.Gateways;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Gateways;

internal class CryptoTickerService : ICryptoTickerService
{
    private readonly ClientWebSocket _socket = new ClientWebSocket();

    public Task ConnectAsync(CancellationToken cancellationToken)
    {
        return _socket.ConnectAsync(new Uri("wss://api.upbit.com/websocket/v1"), cancellationToken);
    }

    public ValueTask SendAsync(string[] codes, CancellationToken cancellationToken)
    {
        var a = new { ticket = Guid.NewGuid().ToString() };
        var b = new { type = "ticker", codes };
        var c = new { format = "DEFAULT" };
        object[] body = [a, b, c];

        string json = JsonSerializer.Serialize(body);
        ReadOnlyMemory<byte> buffer = Encoding.UTF8.GetBytes(json);

        return _socket.SendAsync(buffer, WebSocketMessageType.Binary, true, cancellationToken);
    }

    public async IAsyncEnumerable<ICryptoTickerService.Ticker> ReceiveAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (_socket.State == WebSocketState.Open)
        {
            using var owner = MemoryPool<byte>.Shared.Rent(2048);
            Memory<byte> buffer = owner.Memory;

            ValueWebSocketReceiveResult result = await _socket.ReceiveAsync(buffer, cancellationToken);
            ICryptoTickerService.Ticker ticker = JsonSerializer.Deserialize<ICryptoTickerService.Ticker>(buffer.Span[..result.Count])!;

            yield return ticker;

            await Task.Delay(100, cancellationToken);
        }
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}
