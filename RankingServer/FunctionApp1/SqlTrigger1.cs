using Azure.Messaging.ServiceBus;
using Infrastructure.EFCore;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunctionApp1;

public class SqlTrigger1
{
    private readonly ILogger _logger;
    private readonly ServiceBusSender _sender;

    public SqlTrigger1(ILoggerFactory loggerFactory, ServiceBusSender sender)
    {
        _logger = loggerFactory.CreateLogger<SqlTrigger1>();
        _sender = sender;
    }

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("SqlTrigger1")]
    public async Task Run(
        [SqlTrigger("[dbo].[Orders]", "TradingKing")] IReadOnlyList<SqlChange<OrderModel>> changes,
            FunctionContext context)
    {
        using var batch = await _sender.CreateMessageBatchAsync(context.CancellationToken);
        foreach (var change in changes)
        {
            var msg = new ServiceBusMessage(JsonSerializer.Serialize(change.Item));
            batch.TryAddMessage(msg);
        }
        await _sender.SendMessagesAsync(batch, context.CancellationToken);
    }
}