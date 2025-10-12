using Refit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Gateways;

public interface IPurchaseApi
{
    [Get("/orders/policy")]
    Task<int> GetPolicyAsync(CancellationToken ct);

    [Get("/orders")]
    Task<IEnumerable<PurchaseRes>> GetAllAsync(CancellationToken ct);

    [Post("/orders/buy")]
    Task BuyAsync([Body] PurchaseReq body, CancellationToken ct);

    public record PurchaseRes(string Code, double Quantity, double Price);
    public record PurchaseReq(string Code, double Quantity);
}
