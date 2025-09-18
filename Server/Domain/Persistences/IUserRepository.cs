using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IUserRepository
{
    Task<bool> ExistIdAsync(string userId, CancellationToken ct);
    Task<bool> AddAsync(User user, CancellationToken ct);
    Task<User?> GetAsync(string id, string encryptedPassword, CancellationToken ct);
    Task UpdateTokenAsync(User user, string token, CancellationToken ct);
}
