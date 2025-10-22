using Application.Orchestrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Application.Orchestrations.SeasonService;

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

    [HttpGet]
    public Task<IEnumerable<SeasonRes>> GetSeasonsAsync(CancellationToken ct)
    {
        return _season.GetSeasonsAsync(ct);
    }

    [HttpGet("{seasonId}")]
    public Task<IEnumerable<RankRes>> GetRanksAsync([FromRoute] int seasonId, CancellationToken ct)
    {
        return _season.GetRanksAsync(seasonId, ct);
    }
}
