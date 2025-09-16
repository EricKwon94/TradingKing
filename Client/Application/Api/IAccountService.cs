using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Api;

public interface IAccountService
{
    [Get("/account")]
    Task<FormRes> GetFormAsync(CancellationToken ct);

    [Post("/account")]
    Task RegisterAsync([Body] RegisterReq body, CancellationToken ct);

    public record RegisterReq(string Id, string Nickname, string Password);
    public record FormRes(int MinIdLength, int MaxIdLength, int MinNicknameLength, int MaxNicknameLength, int MinPasswordLength);
}
