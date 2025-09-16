using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IUserRepository
{
    Task<bool> ExistIdAsync(string userId, CancellationToken ct);
    Task<bool> ExistNicknameAsync(string nickname, CancellationToken ct);
    Task<bool> AddAsync(User user, CancellationToken ct);
}
