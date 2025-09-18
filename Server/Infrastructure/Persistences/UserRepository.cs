using Domain;
using Domain.Persistences;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistences;

internal class UserRepository : IUserRepository
{
    private readonly TradingKingContext _context;

    public UserRepository(TradingKingContext context)
    {
        _context = context;
    }

    public Task<bool> ExistIdAsync(string userId, CancellationToken ct)
    {
        return _context.Users.AsNoTracking().AnyAsync(u => u.Id == userId, ct);
    }

    public async Task<bool> AddAsync(User user, CancellationToken ct)
    {
        await _context.Users.AddAsync(user, ct);
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return false;
        }
        return true;
    }

    public Task<User?> GetAsync(string id, string encryptedPassword, CancellationToken ct)
    {
        return _context.Users.AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id && e.Password == encryptedPassword, ct);
    }

    public Task UpdateTokenAsync(User user, string token, CancellationToken ct)
    {
        _context.Users.Attach(user);
        user.Jwt = token;
        return _context.SaveChangesAsync(ct);
    }
}
