using Domain;
using Domain.Persistences;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistences;

internal class UserRepository : IUserRepository
{
    private readonly DbSet<User> _users;

    public UserRepository(TradingKingContext context)
    {
        _users = context.Users;
    }

    public async ValueTask AddAsync(User user, CancellationToken ct)
    {
        await _users.AddAsync(user, ct);
    }

    public Task<User?> GetAsync(string id, string encryptedPassword, CancellationToken ct)
    {
        return _users.AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id && e.Password == encryptedPassword, ct);
    }

    public Task<User> GetUserWithOrderAsync(string id, string code, CancellationToken ct)
    {
        return _users
            .Include(e => e.Orders.Where(e => e.Code == code))
            .SingleAsync(e => e.Id == id, cancellationToken: ct);
    }

    public void UpdateToken(User user, string token)
    {
        _users.Attach(user);
        user.Jwt = token;
    }
}
