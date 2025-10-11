using Domain;
using Domain.Persistences;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orchestrations;

public class PurchaseService
{
    private readonly ITransaction _transaction;
    private readonly IPurchaseRepo _userRepository;

    public PurchaseService(ITransaction transaction, IPurchaseRepo userRepository)
    {
        _transaction = transaction;
        _userRepository = userRepository;
    }

    public Task<List<Purchase>> GetAsync(int userSeq, CancellationToken ct)
    {
        return _userRepository.GetPurchasesAsync(userSeq, ct);
    }
}
