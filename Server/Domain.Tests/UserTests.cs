using Domain.Exceptions;
using FluentAssertions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_user()
    {
        string expectedId = "id13";
        string expectedNickname = "nick1";
        string password = "password1";
        string expectedPassword = Encrypt(password);

        User sut = new User(expectedId, expectedNickname, password);
        sut.Id.Should().Be(expectedId);
        sut.Nickname.Should().Be(expectedNickname);
        sut.Password.Should().Be(expectedPassword);
    }

    [Theory]
    [InlineData("abc1")]
    [InlineData("abcA3")]
    [InlineData("abc123456789012")]
    public void Can_create_user_if_valid_id(string id)
    {
        string expectedNickname = "nick";
        string password = "password1";
        string expectedPassword = Encrypt(password);

        User sut = new User(id, "nick", password);
        sut.Id.Should().Be(id);
        sut.Nickname.Should().Be(expectedNickname);
        sut.Password.Should().Be(expectedPassword);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("abc@")]
    [InlineData("abcA")]
    [InlineData("abc1234567890123")]
    public void Can_not_create_user_if_invalid_id(string id)
    {
        Func<User> sut = () => new User(id, "nick", "password123");
        sut.Should().Throw<InvalidIdException>();
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("123")]
    [InlineData("abcA12")]
    public void Can_create_user_if_valid_nickname(string nickname)
    {
        string expectedId = "Id123";
        string password = "password1";
        string expectedPassword = Encrypt(password);

        User sut = new User(expectedId, nickname, password);
        sut.Id.Should().Be(expectedId);
        sut.Nickname.Should().Be(nickname);
        sut.Password.Should().Be(expectedPassword);
    }

    [Theory]
    [InlineData("b")]
    [InlineData("123@@")]
    [InlineData("abcㄱ12")]
    [InlineData("abcA123")]
    public void Can_not_create_user_if_invalid_nickname(string nickname)
    {
        Func<User> sut = () => new User("Id123", nickname, "password123");
        sut.Should().Throw<InvalidNicknameException>();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("1234a@")]
    public void Can_create_user_if_valid_password(string password)
    {
        string expectedId = "Id123";
        string expectedNickname = "nick";
        string expectedPassword = Encrypt(password);

        User sut = new User(expectedId, expectedNickname, password);
        sut.Id.Should().Be(expectedId);
        sut.Nickname.Should().Be(expectedNickname);
        sut.Password.Should().Be(expectedPassword);
    }

    [Theory]
    [InlineData("12345")]
    public void Can_not_create_user_if_invalid_password(string password)
    {
        Func<User> sut = () => new User("Id123", "nick", password);
        sut.Should().Throw<InvalidPasswordException>();
    }

    private static string Encrypt(string password)
    {
        byte[] encrypt = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(encrypt);
    }
}
