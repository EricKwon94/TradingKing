using System;

namespace Domain.Exceptions;

[Serializable]
public abstract class DomainException : Exception
{
    public abstract int Code { get; }

    internal DomainException() { }
    internal DomainException(string message) : base(message) { }
}
