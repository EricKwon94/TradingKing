namespace Domain.Exceptions;

public class NotEnoughCashException : DomainException
{
    public override int Code => -3;

    internal NotEnoughCashException() { }
    internal NotEnoughCashException(string message) : base(message) { }
}

public class PriceTooLowException : DomainException
{
    public override int Code => -4;

    internal PriceTooLowException() { }
    internal PriceTooLowException(string message) : base(message) { }
}

public class NotEnoughCoinException : DomainException
{
    public override int Code => -5;

    internal NotEnoughCoinException() { }
    internal NotEnoughCoinException(string message) : base(message) { }
}