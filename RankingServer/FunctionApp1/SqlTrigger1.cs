using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FunctionApp1;

public class SqlTrigger1
{
    private readonly ILogger _logger;
    private readonly EventHubFactory _hubFactory;
    private const string CS = "Data Source=tradingking-dev-koreacentral-01.database.windows.net;Initial Catalog=TradingKing;User ID=TCdK8WpDqmknFQVpB7Dev;Password=QM42QSCU^BvQ25mM@0LgDev;";

    public SqlTrigger1(ILoggerFactory loggerFactory, EventHubFactory hubFactory)
    {
        _logger = loggerFactory.CreateLogger<SqlTrigger1>();
        _hubFactory = hubFactory;
    }

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("SqlTrigger1")]
    public void Run(
        [SqlTrigger("[dbo].[Orders]", "TradingKing")] IReadOnlyList<SqlChange<Order>> changes,
            FunctionContext context)
    {
        foreach (SqlChange<Order> change in changes)
        {
            _logger.LogInformation("{item}", change.Item);
        }

        /*await using EventHubProducerClient hub = _hubFactory.CreateSqlHub();

        EventDataBatch batch = await hub.CreateBatchAsync(context.CancellationToken);

        foreach (SqlChange<Order> change in changes)
        {
            _logger.LogInformation("{item}", change.Item);
            var data = new EventData(JsonSerializer.Serialize(change.Item));
            batch.TryAdd(data);
        }

        await hub.SendAsync(batch, context.CancellationToken);*/
    }
}

public record Order(Guid Id, string UserId, string Code, double Quantity, double Price);