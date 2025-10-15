using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Gateways.Hubs;

public interface IChatApi : IChatHub, IAsyncDisposable
{
    Task StartAsync(CancellationToken ct);
    IDisposable ReceiveMessage(Func<string, string, Task> handler);
}
