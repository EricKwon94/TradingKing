using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;

namespace FunctionApp1;

public class SqlTrigger1
{
    private readonly ILogger _logger;

    public SqlTrigger1(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SqlTrigger1>();
    }

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("SqlTrigger1")]
    public void Run(
        [SqlTrigger("[dbo].[table1]", "TradingKing")] IReadOnlyList<SqlChange<ToDoItem>> changes,
            FunctionContext context)
    {
        _logger.LogInformation("SQL Changes: " + JsonSerializer.Serialize(changes));
    }
}

public class ToDoItem
{
    public string Id { get; set; }
    public int Priority { get; set; }
    public string Description { get; set; }
}