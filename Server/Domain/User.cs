using Domain.Exceptions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Domain;

public class User
{
    public const int MIN_ID_LENGTH = 4;
    public const int MAX_ID_LENGTH = 10;
    public const int MIN_PASSWORD_LENGTH = 6;

    public int Seq { get; }
    public string Id { get; }
    public string Password { get; }
    public string? Jwt { get; set; }

    public ICollection<Purchase> Purchases { get; } = [];

    /// <exception cref="InvalidIdException"></exception>
    /// <exception cref="InvalidPasswordException"></exception>
    public User(string id, string password)
    {
        string idPattern = $@"^[가-힣A-Za-z0-9]{{{MIN_ID_LENGTH},{MAX_ID_LENGTH}}}$";
        bool idValid = Regex.IsMatch(id, idPattern);
        if (!idValid)
            throw new InvalidIdException();

        if (password.Length < MIN_PASSWORD_LENGTH)
            throw new InvalidPasswordException();

        Id = id;
        Password = password;

        Purchases.Add(new Purchase(Seq, Purchase.DEFAULT_CODE, 1, Purchase.DEFAULT_PRICE));
    }
}
