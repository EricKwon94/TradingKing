using Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Domain;

public class User
{
    public const int MIN_ID_LENGTH = 4;
    public const int MAX_ID_LENGTH = 10;
    public const int MIN_PASSWORD_LENGTH = 6;

    public int Seq { get; private set; }
    public string Id { get; private set; }
    public string Password { get; private set; }
    public string? Jwt { get; set; }

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
    }
}
