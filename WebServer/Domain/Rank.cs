namespace Domain;

public class Rank
{
    public int SeasonId { get; }
    public string UserId { get; }
    public double Assets { get; }

    public Season? Season { get; }
    public User? User { get; }

    public Rank(int seasonId, string userId, double assets)
    {
        SeasonId = seasonId;
        UserId = userId;
        Assets = assets;
    }
}
