namespace Domain;

public class Order
{
    public const string DEFAULT_CODE = "KRW-CASH";
    public const double DEFAULT_PRICE = 100_000_000;
    public const int MIN_ORDER_PRICE = 10_000;

    public int UserSeq { get; }
    public string Code { get; }
    public double Quantity { get; }
    public double Price { get; }

    public User? User { get; }

    public Order(int userSeq, string code, double quantity, double price)
    {
        UserSeq = userSeq;
        Code = code;
        Quantity = quantity;
        Price = price;
    }
}
