using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IUserRepository
{
    Task<bool> ExistIdAsync(string userId, CancellationToken ct);
    ValueTask AddAsync(User user, CancellationToken ct);
    Task<User?> GetAsync(string id, string encryptedPassword, CancellationToken ct);
    void UpdateToken(User user, string token);
}
