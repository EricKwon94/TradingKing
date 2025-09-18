using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;

namespace Host.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    public static int num = Random.Shared.Next();
    public static Timer timer = new Timer(3000);

    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
        if (!timer.Enabled)
        {
            timer.Elapsed += (s, e) =>
            {
                _logger.LogInformation("{time}: Scale out test {num}", e.SignalTime, num);
            };
            timer.Start();
        }
    }

    [HttpGet]
    public int Test1()
    {
        return num;
    }
}
