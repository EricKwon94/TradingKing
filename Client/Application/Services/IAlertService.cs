using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public interface IAlertService
{
    Task DisplayAlertAsync(string a, string b, string c, CancellationToken ct);
}
