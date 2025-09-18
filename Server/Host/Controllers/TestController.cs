using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Host.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    public static int num = Random.Shared.Next();
    public static Timer timer = new Timer(_ =>
    {
        Console.WriteLine("Scale out test" + num);
    }, null, 1000, 3000);

    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public int Test1()
    {
        return num;
    }
}
