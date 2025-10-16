using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IOrderRepo
{
    Task<List<Order>> GetAllAsync(string userId, CancellationToken ct);
    ValueTask AddAsync(Order purchase, CancellationToken ct);
}
