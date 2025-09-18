using Domain.Exceptions;
using FluentAssertions;
using System;

namespace Domain.Tests;

public class UserTests
{
    [Fact]
    public void Create_user()
    {
        string expectedId = "id13";
        string expectedPassword = "password1";

        User sut = new User(expectedId, expectedPassword);
        sut.Id.Should().Be(expectedId);
        sut.Password.Should().Be(expectedPassword);
    }

    [Theory]
    [InlineData("abca")]
    [InlineData("abcA")]
    [InlineData("abc1")]
    [InlineData("1234")]
    [InlineData("abcA3")]
    [InlineData("가나다라")]
    [InlineData("abc1234567")]
    public void Can_create_user_if_valid_id(string id)
    {
        string expectedPassword = "password1";

        User sut = new User(id, expectedPassword);
        sut.Id.Should().Be(id);
        sut.Password.Should().Be(expectedPassword);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("abc@")]
    [InlineData("abcㄱ")]
    [InlineData("abc12345678")]
    public void Can_not_create_user_if_invalid_id(string id)
    {
        Func<User> sut = () => new User(id, "password123");
        sut.Should().Throw<InvalidIdException>();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("1234a@")]
    public void Can_create_user_if_valid_password(string password)
    {
        string expectedId = "Id123";

        User sut = new User(expectedId, password);
        sut.Id.Should().Be(expectedId);
        sut.Password.Should().Be(password);
    }

    [Theory]
    [InlineData("12345")]
    public void Can_not_create_user_if_invalid_password(string password)
    {
        Func<User> sut = () => new User("Id123", password);
        sut.Should().Throw<InvalidPasswordException>();
    }
}
