using Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Domain;

public class User : IEntity<string>
{
    public const int MIN_ID_LENGTH = 4;
    public const int MAX_ID_LENGTH = 10;
    public const int MIN_PASSWORD_LENGTH = 6;

    public string Id { get; }
    public string Password { get; }
    public string? Jwt { get; set; }

    private readonly List<Order> _orders = [];
    public IReadOnlyList<Order> Orders => _orders;

#pragma warning disable CS8618
    private User()
    {

    }
#pragma warning restore CS8618

    /// <exception cref="InvalidIdException"></exception>
    /// <exception cref="InvalidPasswordException"></exception>
    public User(string id, string password)
    {
        string idPattern = $@"^[가-힣A-Za-z0-9]{{{MIN_ID_LENGTH},{MAX_ID_LENGTH}}}$";
        bool idValid = Regex.IsMatch(id, idPattern);
        if (!idValid)
            throw new InvalidIdException();

        if (id.Contains("GM") || id.Contains("운영자"))
            throw new InvalidIdException();

        if (password.Length < MIN_PASSWORD_LENGTH)
            throw new InvalidPasswordException();

        Id = id;
        Password = password;

        _orders.Add(new Order(Id, Order.DEFAULT_CODE, Order.DEFAULT_PRICE, 1));
    }

    /// <exception cref="PriceTooLowException"></exception>
    /// <exception cref="NotEnoughCashException"></exception>
    public void BuyCoin(string code, double buyQuantity, double tickerPrice)
    {
        double price = buyQuantity * tickerPrice;
        if (price < Order.MIN_ORDER_PRICE)
            throw new PriceTooLowException();

        double availableCash = Orders.Where(e => e.Code == Order.DEFAULT_CODE).Sum(e => e.Quantity);
        if (availableCash < price)
            throw new NotEnoughCashException();

        var cryto = new Order(Id, code, buyQuantity, tickerPrice);
        var cash = new Order(Id, Order.DEFAULT_CODE, price * -1, 1);
        _orders.AddRange(cryto, cash);
    }

    /// <exception cref="PriceTooLowException"></exception>
    /// <exception cref="NotEnoughCoinException"></exception>
    public void SellCoin(string code, double sellQuantity, double tickerPrice)
    {
        double quantity = Orders.Where(e => e.Code == code).Sum(e => e.Quantity);
        if (sellQuantity > quantity)
            throw new NotEnoughCoinException();

        double price = sellQuantity * tickerPrice;
        if (price < Order.MIN_ORDER_PRICE)
            throw new PriceTooLowException();

        var cryto = new Order(Id, code, sellQuantity * -1, tickerPrice);
        var cash = new Order(Id, Order.DEFAULT_CODE, price, 1);
        _orders.AddRange(cryto, cash);
    }
}
