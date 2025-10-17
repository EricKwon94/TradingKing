using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;

namespace FunctionApp1;

public class TimerTrigger1
{
    private readonly ILogger _logger;
    private readonly ServiceBusSender _sender;

    public TimerTrigger1(ILoggerFactory loggerFactory, ServiceBusSender sender)
    {
        _logger = loggerFactory.CreateLogger<TimerTrigger1>();
        _sender = sender;
    }


    //[Function("TimerTrigger1")]
    public void Run(
      [TimerTrigger("*/50 * * * * *")] TimerInfo myTimer, FunctionContext context)
    {
        _logger.LogInformation("타이머트리거 {time}", DateTime.UtcNow);
    }
}