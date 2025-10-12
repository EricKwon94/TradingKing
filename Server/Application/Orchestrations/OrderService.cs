using Application.Gateways;
using Domain;
using Domain.Exceptions;
using Domain.Persistences;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Orchestrations;

public class OrderService
{
    private readonly ITransaction _transaction;
    private readonly IOrderRepo _orderRepo;
    private readonly IExchangeApi _exchangeApi;

    public OrderService(ITransaction transaction, IOrderRepo orderRepo, IExchangeApi exchangeApi)
    {
        _transaction = transaction;
        _orderRepo = orderRepo;
        _exchangeApi = exchangeApi;
    }

    public int GetPolicy()
    {
        return Order.MIN_ORDER_PRICE;
    }

    public async Task<IEnumerable<OrderRes>> GetAllAsync(int userSeq, CancellationToken ct)
    {
        var orders = await _orderRepo.GetAllAsync(userSeq, ct);
        return orders.Select(e => new OrderRes(e.Code, e.Quantity, e.Price));
    }

    public async Task<double> GetCashAsync(int userSeq, CancellationToken ct)
    {
        var orders = await _orderRepo.GetAllAsync(userSeq, Order.DEFAULT_CODE, ct);
        return orders.Sum(e => e.Price);
    }

    /// <exception cref="PriceTooLowException"></exception>
    /// <exception cref="NotEnoughCashException"></exception>
    public async Task BuyAsync(int userSeq, OrderReq req, CancellationToken ct)
    {
        var tickers = await _exchangeApi.GetTickerAsync(req.Code, ct);
        var ticker = tickers.Single();

        double price = req.Quantity * ticker.trade_price;
        if (price < Order.MIN_ORDER_PRICE)
            throw new PriceTooLowException();

        double availableCash = await GetCashAsync(userSeq, ct);
        if (availableCash < price)
            throw new NotEnoughCashException();

        var cryto = new Order(userSeq, req.Code, req.Quantity, ticker.trade_price);
        var cash = new Order(userSeq, Order.DEFAULT_CODE, 1, price * -1);
        await _orderRepo.AddAsync(cryto, ct);
        await _orderRepo.AddAsync(cash, ct);

        await _transaction.SaveChangesAsync(ct);
    }

    /// <exception cref="PriceTooLowException"></exception>
    /// <exception cref="NotEnoughCoinException"></exception>
    public async Task SellAsync(int userSeq, OrderReq req, CancellationToken ct)
    {
        var orders = await _orderRepo.GetAllAsync(userSeq, req.Code, ct);
        var quantity = orders.Sum(e => e.Quantity);

        if (req.Quantity > quantity)
            throw new NotEnoughCoinException();

        var tickers = await _exchangeApi.GetTickerAsync(req.Code, ct);
        var ticker = tickers.Single();

        double price = req.Quantity * ticker.trade_price;

        if (price < Order.MIN_ORDER_PRICE)
            throw new PriceTooLowException();

        var cryto = new Order(userSeq, req.Code, req.Quantity * -1, ticker.trade_price);
        var cash = new Order(userSeq, Order.DEFAULT_CODE, 1, price);
        await _orderRepo.AddAsync(cryto, ct);
        await _orderRepo.AddAsync(cash, ct);

        await _transaction.SaveChangesAsync(ct);
    }

    public record OrderRes(string Code, double Quantity, double Price);
    public record OrderReq(string Code, double Quantity);
}
