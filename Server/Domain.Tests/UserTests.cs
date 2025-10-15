using Domain.Exceptions;
using FluentAssertions;
using System;
using System.Linq;

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
    [InlineData("GM12")]
    [InlineData("운영자12")]
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

    [Fact]
    public void Buy_coin()
    {
        // arrange
        string expectedCode = "KRW-DOGE";
        double expectedQuantity = 100;
        double expectedPrice = 300;
        double expectedCash = 100_000_000 - expectedQuantity * expectedPrice;

        User sut = new User("zxcxzs21", "asfhlkidsgvhl!");

        // act
        sut.BuyCoin(expectedCode, expectedQuantity, expectedPrice);

        // assert
        var cashes = sut.Orders.Where(e => e.Code == "KRW-CASH").ToList();
        cashes.Count.Should().Be(2);
        cashes.Sum(e => e.Price).Should().Be(expectedCash);

        var doge = sut.Orders.Single(e => e.Code == "KRW-DOGE");
        doge.Code.Should().Be(expectedCode);
        doge.Quantity.Should().Be(expectedQuantity);
        doge.Price.Should().Be(expectedPrice);
    }

    [Fact]
    public void Can_not_buy_coin_if_not_enough_cash()
    {
        User sut = new User("zxcxzs21", "asfhlkidsgvhl!");
        Action testCode = () => sut.BuyCoin("KRW-BTC", 1, 177_000_000);
        Assert.Throws<NotEnoughCashException>(testCode);
    }

    [Fact]
    public void Can_not_buy_coin_if_not_order_price_too_low()
    {
        User sut = new User("zxcxzs21", "asfhlkidsgvhl!");
        Action testCode = () => sut.BuyCoin("KRW-DOGE", 1, 300);
        Assert.Throws<PriceTooLowException>(testCode);
    }

    [Fact]
    public void Sell_coin()
    {
        // arrange
        string expectedCode = "KRW-DOGE";
        double expectedRemainingQuantity = 2;
        double expectedQuantity = 100;
        double expectedPrice = 300;
        double expectedCash = 100_000_000 - expectedRemainingQuantity * expectedPrice;

        User sut = new User("zxcxzs21", "asfhlkidsgvhl!");
        sut.BuyCoin(expectedCode, expectedQuantity + expectedRemainingQuantity, expectedPrice);

        // act
        sut.SellCoin(expectedCode, expectedQuantity, expectedPrice);

        // assert
        var cashes = sut.Orders.Where(e => e.Code == "KRW-CASH").ToList();
        cashes.Count.Should().Be(3);
        cashes.Sum(e => e.Price).Should().Be(expectedCash);

        var doges = sut.Orders.Where(e => e.Code == "KRW-DOGE").ToList();
        doges.Count.Should().Be(2);
        doges[0].Code.Should().Be(expectedCode);
        doges[0].Quantity.Should().Be(expectedQuantity + expectedRemainingQuantity);
        doges[0].Price.Should().Be(expectedPrice);
        doges[1].Code.Should().Be(expectedCode);
        doges[1].Quantity.Should().Be(expectedQuantity * -1);
        doges[1].Price.Should().Be(expectedPrice);
    }

    [Fact]
    public void Can_not_sell_coin_if_not_enough_coin()
    {
        User sut = new User("zxcxzs21", "asfhlkidsgvhl!");
        Action testCode = () => sut.SellCoin("KRW-BTC", 1, 177_000_000);
        Assert.Throws<NotEnoughCoinException>(testCode);
    }

    [Fact]
    public void Can_not_sell_coin_if_not_order_price_too_low()
    {
        User sut = new User("zxcxzs21", "asfhlkidsgvhl!");
        sut.BuyCoin("KRW-DOGE", 100, 300);
        Action testCode = () => sut.SellCoin("KRW-DOGE", 1, 300);
        Assert.Throws<PriceTooLowException>(testCode);
    }
}
