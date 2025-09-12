using Refit;
using System.Threading.Tasks;

namespace Application.Api;

public interface IAccountService
{
    [Get("/login")]
    Task<string> Login();

    [Get("/login/version")]
    Task<int> Version();

    [Get("/login/user")]
    Task<string> User();

    [Get("/login/admin")]
    Task<string> Admin();
}
