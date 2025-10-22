using Domain.Persistences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orchestrations;

public class SeasonService
{
    private readonly ISeasonRepo _seasonRepo;
    private readonly IRankRepo _rankRepo;

    public SeasonService(ISeasonRepo seasonRepo, IRankRepo rankRepo)
    {
        _seasonRepo = seasonRepo;
        _rankRepo = rankRepo;
    }

    public async Task<IEnumerable<SeasonRes>> GetSeasonsAsync(CancellationToken ct)
    {
        var seasons = await _seasonRepo.GetSeasonsAsync(ct);
        return seasons.Select(e => new SeasonRes(e.Id, e.StartedAt));
    }

    public async Task<IEnumerable<RankRes>> GetRanksAsync(int seasonId, CancellationToken ct)
    {
        var ranks = await _rankRepo.GetRanksAsync(seasonId, ct);
        return ranks.Select(e => new RankRes(e.SeasonId, e.UserId, e.Assets));
    }

    public record SeasonRes(int Id, DateTime StartedAt);
    public record RankRes(int SeasonId, string UserId, double Assets);
}
