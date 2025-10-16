using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

string eventHubCs = builder.Configuration.GetConnectionString("EventHub")!;
string sqlHubName = builder.Configuration["SqlHubName"]!;
string timerHubName = builder.Configuration["TimerHubName"]!;

builder.Services.AddSingleton((p) => new EventHubFactory(eventHubCs, sqlHubName, timerHubName));

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();

public class EventHubFactory
{
    private readonly string _eventHubCs;
    private readonly string _sqlHubName;
    private readonly string _timerHubName;

    public EventHubFactory(string eventHubCs, string sqlHubName, string timerHubName)
    {
        _eventHubCs = eventHubCs;
        _sqlHubName = sqlHubName;
        _timerHubName = timerHubName;
    }

    public EventHubProducerClient CreateSqlHub()
    {
        return new EventHubProducerClient(_eventHubCs, _sqlHubName);
    }

    public EventHubProducerClient CreateTimerHub()
    {
        return new EventHubProducerClient(_eventHubCs, _timerHubName);
    }
}