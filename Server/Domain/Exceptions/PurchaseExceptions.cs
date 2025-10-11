namespace Domain.Exceptions;

public class NotEnoughCashException : DomainException
{
    public override int Code => -3;

    public NotEnoughCashException() { }
    public NotEnoughCashException(string message) : base(message) { }
}