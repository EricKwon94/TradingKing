using Application.Gateways;
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
    private readonly IExchangeApi _exchangeApi;

    public PurchaseService(ITransaction transaction, IPurchaseRepo purchaseRepo, IExchangeApi exchangeApi)
    {
        _transaction = transaction;
        _purchaseRepo = purchaseRepo;
        _exchangeApi = exchangeApi;
    }

    public int GetPolicy()
    {
        return Purchase.MIN_ORDER_PRICE;
    }

    public async Task<IEnumerable<PurchaseRes>> GetAllAsync(int userSeq, CancellationToken ct)
    {
        var purchases = await _purchaseRepo.GetAllAsync(userSeq, ct);
        return purchases.Select(e => new PurchaseRes(e.Code, e.Quantity, e.Price));
    }

    public async Task<double> GetCashAsync(int userSeq, CancellationToken ct)
    {
        var purchases = await GetAllAsync(userSeq, ct);
        return purchases.Where(e => e.Code == Purchase.DEFAULT_CODE).Sum(e => e.Price);
    }

    /// <exception cref="PriceTooLowException"></exception>
    /// <exception cref="NotEnoughCashException"></exception>
    public async Task BuyAsync(int userSeq, PurchaseReq req, CancellationToken ct)
    {
        var tickers = await _exchangeApi.GetTickerAsync(req.Code, ct);
        var ticker = tickers.Single();

        double availableCash = await GetCashAsync(userSeq, ct);
        double price = req.Quantity * ticker.trade_price;

        if (price < Purchase.MIN_ORDER_PRICE)
            throw new PriceTooLowException();

        if (availableCash < price)
            throw new NotEnoughCashException();

        var cryto = new Purchase(userSeq, req.Code, req.Quantity, ticker.trade_price);
        var cash = new Purchase(userSeq, Purchase.DEFAULT_CODE, 1, price * -1);
        await _purchaseRepo.AddAsync(cryto, ct);
        await _purchaseRepo.AddAsync(cash, ct);

        await _transaction.SaveChangesAsync(ct);
    }

    public record PurchaseRes(string Code, double Quantity, double Price);
    public record PurchaseReq(string Code, double Quantity);
}
