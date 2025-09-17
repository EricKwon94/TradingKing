using Domain;
using Domain.Exceptions;
using Domain.Persistences;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Application;

public class AccountService
{
    private readonly IUserRepository _userRepository;

    public AccountService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public FormRes GetForm()
    {
        return new FormRes(
            User.MIN_ID_LENGTH, User.MAX_ID_LENGTH, User.MIN_NICKNAME_LENGTH,
            User.MAX_NICKNAME_LENGTH, User.MIN_PASSWORD_LENGTH);
    }

    /// <exception cref="InvalidIdException"></exception>
    /// <exception cref="InvalidNicknameException"></exception>
    /// <exception cref="InvalidPasswordException"></exception>
    public async Task<RegisterResult> RegisterAsync(string id, string nickname, string password, CancellationToken ct)
    {
        var user = new User(id, nickname, password);

        bool existId = await _userRepository.ExistIdAsync(id, ct);
        if (existId)
            return RegisterResult.DuplicateId;

        bool existNickname = await _userRepository.ExistNicknameAsync(nickname, ct);
        if (existNickname)
            return RegisterResult.DuplicateNickname;

        bool succ = await _userRepository.AddAsync(user, ct);
        if (!succ)
            return RegisterResult.DuplicateAccount;

        return RegisterResult.Ok;
    }

    public record FormRes(int MinIdLength, int MaxIdLength, int MinNicknameLength, int MaxNicknameLength, int MinPasswordLength);

    public record RegisterReq(
        [MinLength(User.MIN_ID_LENGTH)][MaxLength(User.MAX_ID_LENGTH)] string Id,
        [MinLength(User.MIN_NICKNAME_LENGTH)][MaxLength(User.MAX_NICKNAME_LENGTH)] string Nickname,
        [MinLength(User.MIN_PASSWORD_LENGTH)] string Password);

    public enum RegisterResult { Ok, DuplicateId, DuplicateNickname, DuplicateAccount }
}
