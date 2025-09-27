using Common;
using Domain;
using Domain.Exceptions;
using Domain.Persistences;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orchestrations;

public class AccountService
{
    private readonly ITransaction _transaction;
    private readonly IUserRepository _userRepository;
    private readonly TokenGenerator _tokenGenerator = new();
    private readonly Encryptor _encryptor = new();

    public AccountService(ITransaction transaction, IUserRepository userRepository)
    {
        _transaction = transaction;
        _userRepository = userRepository;
    }

    public FormRes GetForm()
    {
        return new FormRes(
            User.MIN_ID_LENGTH, User.MAX_ID_LENGTH, User.MIN_PASSWORD_LENGTH);
    }

    /// <exception cref="InvalidIdException"></exception>
    /// <exception cref="InvalidPasswordException"></exception>
    public async Task<bool> RegisterAsync(string id, string password, CancellationToken ct)
    {
        string encrypted = _encryptor.Encrypt(password);
        var user = new User(id, encrypted);

        await _userRepository.AddAsync(user, ct);
        try
        {
            await _transaction.SaveChangesAsync(ct);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<string?> LoginAsync(
        string id, string password, string issKey, string iss, string aud,
        CancellationToken ct)
    {
        string encrypted = _encryptor.Encrypt(password);
        var user = await _userRepository.GetAsync(id, encrypted, ct);
        if (user == null)
            return null;

        string jwt = _tokenGenerator.CreateJwt(user.Seq.ToString(), issKey, iss, aud);
        _userRepository.UpdateToken(user, jwt);
        await _transaction.SaveChangesAsync(ct);
        return jwt;
    }

    public record FormRes(int MinIdLength, int MaxIdLength, int MinPasswordLength);

    public record RegisterReq(
        [MinLength(User.MIN_ID_LENGTH)][MaxLength(User.MAX_ID_LENGTH)] string Id,
        [MinLength(User.MIN_PASSWORD_LENGTH)] string Password);

    public record LoginReq(
        [MinLength(User.MIN_ID_LENGTH)][MaxLength(User.MAX_ID_LENGTH)] string Id,
        [MinLength(User.MIN_PASSWORD_LENGTH)] string Password);
}
