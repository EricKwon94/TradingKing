using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Persistences;

public interface IOrderRepo
{
    Task<List<Order>> GetAllAsync(int userSeq, CancellationToken ct);
    Task<List<Order>> GetAllAsync(int userSeq, string code, CancellationToken ct);
    ValueTask AddAsync(Order purchase, CancellationToken ct);
}
