using Domain;
using Domain.Persistences;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistences;

internal class RankRepo : IRankRepo
{
    private readonly DbSet<Rank> _ranks;

    public RankRepo(TradingKingContext context)
    {
        _ranks = context.Ranks;
    }

    public Task<List<Rank>> GetRanksAsync(int seasonId, CancellationToken ct)
    {
        return _ranks.AsNoTracking()
            .Where(e => e.SeasonId == seasonId)
            .ToListAsync(ct);
    }
}
