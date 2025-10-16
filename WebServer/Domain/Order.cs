using System;

namespace Domain;

public class Order : IEntity<Guid>
{
    public const string DEFAULT_CODE = "KRW-CASH";
    public const double DEFAULT_PRICE = 100_000_000;
    public const int MIN_ORDER_PRICE = 10_000;

    public Guid Id { get; }
    public string UserId { get; }
    public string Code { get; }
    public double Quantity { get; }
    public double Price { get; }

    public User? User { get; }

    public Order(string userId, string code, double quantity, double price)
    {
        UserId = userId;
        Code = code;
        Quantity = quantity;
        Price = price;
    }
}
