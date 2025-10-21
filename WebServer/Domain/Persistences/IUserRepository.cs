using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IUserRepository
{
    ValueTask AddAsync(User user, CancellationToken ct);
    Task<User?> GetAsync(string id, string encryptedPassword, CancellationToken ct);
    Task<User> GetUserWithOrderAsync(int seasonId, string id, string code, CancellationToken ct);
    void UpdateToken(User user, string token);
}
