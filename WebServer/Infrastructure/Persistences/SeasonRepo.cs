using Domain;
using Domain.Persistences;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistences;

internal class SeasonRepo : ISeasonRepo
{
    private readonly DbSet<Season> _seasons;

    public SeasonRepo(TradingKingContext context)
    {
        _seasons = context.Seasons;
    }

    public async Task<int> GetLastSeasonIdAsync(CancellationToken ct)
    {
        var season = await _seasons.AsNoTracking().MaxAsync(e => e.Id);
        if (season == 0)
            throw new NullReferenceException("DB에 시즌은 반드시 하나 이상 설정되어 야한다.");
        return season;
    }
}
