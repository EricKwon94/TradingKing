using System.Threading.Tasks;

namespace Application.Gateways;

public interface IChatHub
{
    Task BroadcastMessage(string name, string message);
    Task Echo(string name, string message);
}

public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
}
