using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public interface INavigationService
{
    Task GoToAsync(string uri, CancellationToken ct);
}
