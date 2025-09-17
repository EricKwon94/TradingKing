using Domain.Exceptions;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain;

public class User
{
    public const int MIN_ID_LENGTH = 4;
    public const int MAX_ID_LENGTH = 15;
    public const int MIN_NICKNAME_LENGTH = 2;
    public const int MAX_NICKNAME_LENGTH = 6;
    public const int MIN_PASSWORD_LENGTH = 6;

    public int Seq { get; private set; }
    public string Id { get; private set; }
    public string Nickname { get; private set; }
    public string Password { get; private set; }

    /// <exception cref="InvalidIdException"></exception>
    /// <exception cref="InvalidNicknameException"></exception>
    /// <exception cref="InvalidPasswordException"></exception>
    public User(string id, string nickname, string password)
    {
        string idPattern = $@"^[A-Za-z0-9]{{{MIN_ID_LENGTH},{MAX_ID_LENGTH}}}$";
        bool idValid = Regex.IsMatch(id, idPattern);
        if (!idValid)
            throw new InvalidIdException();

        string nicknamePattern = $@"^[가-힣A-Za-z0-9]{{{MIN_NICKNAME_LENGTH},{MAX_NICKNAME_LENGTH}}}$";
        bool nicknameValid = Regex.IsMatch(nickname, nicknamePattern);
        if (!nicknameValid)
            throw new InvalidNicknameException();

        if (password.Length < MIN_PASSWORD_LENGTH)
            throw new InvalidPasswordException();

        Id = id;
        Nickname = nickname;
        byte[] encrypt = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        Password = Convert.ToHexString(encrypt);
    }
}
