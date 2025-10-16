using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FunctionApp1;

public class TimerTrigger1
{
    private readonly ILogger _logger;
    private readonly EventHubFactory _hubFactory;

    public TimerTrigger1(ILoggerFactory loggerFactory, EventHubFactory hubFactory)
    {
        _logger = loggerFactory.CreateLogger<TimerTrigger1>();
        _hubFactory = hubFactory;
    }


    //  [Function("TimerTrigger1")]
    public async Task Run(
        [TimerTrigger("*/55 * * * * *")] TimerInfo myTimer, CancellationToken ct)
    {
        await using EventHubProducerClient hub = _hubFactory.CreateSqlHub();

        EventDataBatch batch = await hub.CreateBatchAsync(ct);
        var data = new EventData($"Hello I am Functions {DateTime.UtcNow}");
        batch.TryAdd(data);
        await hub.SendAsync(batch, ct);
    }
}