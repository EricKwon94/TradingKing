using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FunctionApp1;

public class SqlTrigger1
{
    private readonly ILogger _logger;
    private readonly ServiceBusSender _orderSender;
    private readonly ServiceBusSender _seasonSender;

    public SqlTrigger1(ILoggerFactory loggerFactory,
        [FromKeyedServices("order")] ServiceBusSender orderSender,
        [FromKeyedServices("rank")] ServiceBusSender seasonSender
        )
    {
        _logger = loggerFactory.CreateLogger<SqlTrigger1>();
        _orderSender = orderSender;
        _seasonSender = seasonSender;
    }

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("SqlTrigger1")]
    public async Task Run(
        [SqlTrigger("[dbo].[Orders]", "TradingKing")] IReadOnlyList<SqlChange<OrderModel>> changes,
            FunctionContext context)
    {
        using var batch = await _orderSender.CreateMessageBatchAsync(context.CancellationToken);
        foreach (var change in changes)
        {
            var msg = new ServiceBusMessage(JsonSerializer.Serialize(change.Item));
            batch.TryAddMessage(msg);
        }
        await _orderSender.SendMessagesAsync(batch, context.CancellationToken);
    }

    [Function("SqlTrigger2")]
    public async Task Run2(
        [SqlTrigger("[dbo].[Seasons]", "TradingKing")] IReadOnlyList<SqlChange<SeasonModel>> changes,
            FunctionContext context)
    {
        using var batch = await _seasonSender.CreateMessageBatchAsync(context.CancellationToken);
        foreach (var change in changes.Where(e => e.Operation == SqlChangeOperation.Insert))
        {
            var msg = new ServiceBusMessage(JsonSerializer.Serialize(change.Item));
            batch.TryAddMessage(msg);
        }

        if (batch.Count > 0)
            await _seasonSender.SendMessagesAsync(batch, context.CancellationToken);
    }
}