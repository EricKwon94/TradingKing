using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Gateways;

public interface IOrderApi
{
    [Get("/orders/policy")]
    Task<int> GetPolicyAsync(CancellationToken ct);

    [Get("/orders")]
    Task<IEnumerable<OrderRes>> GetAllAsync(CancellationToken ct);

    [Post("/orders/buy")]
    Task BuyAsync([Body] OrderReq body, CancellationToken ct);

    [Post("/orders/sell")]
    Task SellAsync([Body] OrderReq body, CancellationToken ct);

    public record OrderRes(string Code, double Quantity, double Price);
    public record OrderReq(string Code, double Quantity);
}
