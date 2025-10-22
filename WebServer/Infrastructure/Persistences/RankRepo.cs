using Domain;
using Domain.Persistences;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences;

internal class RankRepo : IRankRepo
{
    private readonly DbSet<Rank> _ranks;

    public RankRepo(TradingKingContext context)
    {
        _ranks = context.Ranks;
    }
}
