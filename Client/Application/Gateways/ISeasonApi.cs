using Refit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Gateways;

public interface ISeasonApi
{
    [Get("/seasons")]
    Task<IEnumerable<SeasonRes>> GetSeasonsAsync(CancellationToken ct);

    [Get("/seasons/{seasonId}")]
    Task<IEnumerable<RankRes>> GetRanksAsync(int seasonId, CancellationToken ct);

    public record SeasonRes(int Id, DateTime StartedAt);
    public record RankRes(int SeasonId, string UserId, double Assets);
}
