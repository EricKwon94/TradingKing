using Domain.Persistences;

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
}
