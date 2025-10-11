namespace Domain;

public class Purchase
{
    public int UserSeq { get; }
    public string Code { get; }
    public double Quantity { get; }
    public double Price { get; }

    public User? User { get; }

    public Purchase(int userSeq, string code, double quantity, double price)
    {
        UserSeq = userSeq;
        Code = code;
        Quantity = quantity;
        Price = price;
    }
}
