using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Api;

public interface IAccountService
{
    [Get("/account")]
    Task<FormRes> GetFormAsync(CancellationToken ct);

    [Post("/account/register")]
    Task RegisterAsync([Body] RegisterReq body, CancellationToken ct);

    [Post("/account/login")]
    Task<string> LoginAsync([Body] LoginReq body, CancellationToken ct);

    public record RegisterReq(string Id, string Nickname, string Password);
    public record LoginReq(string Id, string Password);
    public record FormRes(int MinIdLength, int MaxIdLength, int MinNicknameLength, int MaxNicknameLength, int MinPasswordLength);
}
