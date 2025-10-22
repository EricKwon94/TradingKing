using Application.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentaion.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class SeasonsController : ControllerBase
{
    private readonly SeasonService _season;

    public SeasonsController(SeasonService season)
    {
        _season = season;
    }
}
