using System.Threading.Tasks;

namespace Application.Services;

public interface IAlertService
{
    Task DisplayAlert(string a, string b, string c);
}
