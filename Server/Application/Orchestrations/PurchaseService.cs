using Domain;
using Domain.Exceptions;
using Domain.Persistences;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orchestrations;

public class PurchaseService
{
    private readonly ITransaction _transaction;
    private readonly IPurchaseRepo _purchaseRepo;

    public PurchaseService(ITransaction transaction, IPurchaseRepo purchaseRepo)
    {
        _transaction = transaction;
        _purchaseRepo = purchaseRepo;
    }

    public Task<List<Purchase>> GetAllAsync(int userSeq, CancellationToken ct)
    {
        return _purchaseRepo.GetAllAsync(userSeq, ct);
    }

    public async Task<double> GetCashAsync(int userSeq, CancellationToken ct)
    {
        var purchases = await GetAllAsync(userSeq, ct);
        return purchases.Where(e => e.Code == Purchase.DEFAULT_CODE).Sum(e => e.Price);
    }

    /// <exception cref="NotEnoughCashException"></exception>
    public async Task BuyAsync(PurchaseReq req, CancellationToken ct)
    {
        double availableCash = await GetCashAsync(req.UserSeq, ct);
        double price = req.Quantity * req.Price;
        if (availableCash < price)
            throw new NotEnoughCashException();

        var cryto = new Purchase(req.UserSeq, req.Code, req.Quantity, req.Price);
        var cash = new Purchase(req.UserSeq, Purchase.DEFAULT_CODE, 1, price * -1);
        await _purchaseRepo.AddAsync(cryto, ct);
        await _purchaseRepo.AddAsync(cash, ct);

        await _transaction.SaveChangesAsync(ct);
    }

    public record PurchaseReq(int UserSeq, string Code, double Quantity, double Price);
}
