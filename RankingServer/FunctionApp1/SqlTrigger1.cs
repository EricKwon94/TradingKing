using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunctionApp1;

public class SqlTrigger1
{
    private readonly ILogger _logger;
    private readonly EventHubFactory _hubFactory;

    public SqlTrigger1(ILoggerFactory loggerFactory, EventHubFactory hubFactory)
    {
        _logger = loggerFactory.CreateLogger<SqlTrigger1>();
        _hubFactory = hubFactory;
    }

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("SqlTrigger1")]
    public async Task Run(
        [SqlTrigger("[dbo].[Orders]", "TradingKing")] IReadOnlyList<SqlChange<Order>> changes,
            FunctionContext context)
    {
        await using EventHubProducerClient hub = _hubFactory.CreateSqlHub();

        EventDataBatch batch = await hub.CreateBatchAsync(context.CancellationToken);

        foreach (SqlChange<Order> change in changes)
        {
            var data = new EventData(JsonSerializer.Serialize(change.Item));
            batch.TryAdd(data);
        }

        await hub.SendAsync(batch, context.CancellationToken);
    }
}

public record Order(Guid Id, string UserId, string Code, double Quantity, double Price);