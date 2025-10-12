namespace Domain.Exceptions;

public class NotEnoughCashException : DomainException
{
    public override int Code => -3;

    public NotEnoughCashException() { }
    public NotEnoughCashException(string message) : base(message) { }
}

public class PriceTooLowException : DomainException
{
    public override int Code => -4;

    public PriceTooLowException() { }
    public PriceTooLowException(string message) : base(message) { }
}

public class NotEnoughCoinException : DomainException
{
    public override int Code => -5;

    public NotEnoughCoinException() { }
    public NotEnoughCoinException(string message) : base(message) { }
}