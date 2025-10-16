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
    private readonly IUserRepository _userRepo;
    private readonly IExchangeApi _exchangeApi;

    public OrderService(ITransaction transaction, IOrderRepo orderRepo, IUserRepository userRepo, IExchangeApi exchangeApi)
    {
        _transaction = transaction;
        _orderRepo = orderRepo;
        _userRepo = userRepo;
        _exchangeApi = exchangeApi;
    }

    public int GetPolicy()
    {
        return Order.MIN_ORDER_PRICE;
    }

    public async Task<IEnumerable<OrderRes>> GetAllAsync(string userId, CancellationToken ct)
    {
        var orders = await _orderRepo.GetAllAsync(userId, ct);
        return orders.Select(e => new OrderRes(e.Code, e.Quantity, e.Price));
    }

    /// <exception cref="PriceTooLowException"></exception>
    /// <exception cref="NotEnoughCashException"></exception>
    public async Task BuyAsync(string userId, OrderReq req, CancellationToken ct)
    {
        var tickers = await _exchangeApi.GetTickerAsync(req.Code, ct);
        var ticker = tickers.Single();

        User user = await _userRepo.GetUserWithOrderAsync(userId, Order.DEFAULT_CODE, ct);
        user.BuyCoin(req.Code, req.Quantity, ticker.trade_price);

        await _transaction.SaveChangesAsync(ct);
    }

    /// <exception cref="PriceTooLowException"></exception>
    /// <exception cref="NotEnoughCoinException"></exception>
    public async Task SellAsync(string userId, OrderReq req, CancellationToken ct)
    {
        var tickers = await _exchangeApi.GetTickerAsync(req.Code, ct);
        var ticker = tickers.Single();

        User user = await _userRepo.GetUserWithOrderAsync(userId, req.Code, ct);
        user.SellCoin(req.Code, req.Quantity, ticker.trade_price);

        await _transaction.SaveChangesAsync(ct);
    }

    public record OrderRes(string Code, double Quantity, double Price);
    public record OrderReq(string Code, double Quantity);
}
