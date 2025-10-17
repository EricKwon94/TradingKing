using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

string serviceBusCs = builder.Configuration["ServiceBus"]!;
string orderQueueName = builder.Configuration["OrderQueueName"]!;
string rankQueueName = builder.Configuration["RankQueueName"]!;

builder.Services.AddSingleton(p => new ServiceBusClient(serviceBusCs));
builder.Services.AddSingleton(p =>
{
    var client = p.GetRequiredService<ServiceBusClient>();
    return client.CreateSender(orderQueueName);
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();