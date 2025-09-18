using System;

namespace Domain.Exceptions;

[Serializable]
public class InvalidIdException : DomainException
{
    public override int Code => -1;

    internal InvalidIdException() { }
    internal InvalidIdException(string message) : base(message) { }
}

[Serializable]
public class InvalidPasswordException : DomainException
{
    public override int Code => -2;

    internal InvalidPasswordException() { }
    internal InvalidPasswordException(string message) : base(message) { }
}

