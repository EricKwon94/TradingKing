using Application.Gateways;
using Application.Orchestrations;
using Domain;
using Domain.Exceptions;
using FluentAssertions;
using Infrastructure.EFCore;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Tests;

public class OrderServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public OrderServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "돈이 부족하면 코인을 살 수 없어")]
    public async Task Test1()
    {
        // arrange
        string id = _fixture.IndependentId;
        using var context = _fixture.CreateContext();
        var user = await RegisterAccountAsync(id, context);

        var req = new OrderService.OrderReq("KRW-BTC", 1);
        var sut = CreateServiceAsync("KRW-BTC", 170_000_000, context);

        // act
        Func<Task> action = () => sut.BuyAsync(user.Seq, req, default);

        // assert
        await action.Should().ThrowAsync<NotEnoughCashException>();
    }

    [Fact(DisplayName = "최소 구매 금액을 넘겨야 합니다.")]
    public async Task Test2()
    {
        // arrange
        string id = _fixture.IndependentId;
        using var context = _fixture.CreateContext();
        var user = await RegisterAccountAsync(id, context);

        var req = new OrderService.OrderReq("KRW-DOGE", 1);
        var sut = CreateServiceAsync("KRW-DOGE", 280, context);

        // act
        Func<Task> action = () => sut.BuyAsync(user.Seq, req, default);

        // assert
        await action.Should().ThrowAsync<PriceTooLowException>();
    }

    [Theory(DisplayName = "코인을 구매 한다.")]
    [InlineData("KRW-DOGE", 110.5, 301)]
    public async Task Test3(string expectedCode, double expectedQuantity, double expectedPrice)
    {
        // arrange
        string id = _fixture.IndependentId;
        using var context = _fixture.CreateContext();
        var user = await RegisterAccountAsync(id, context);
        var expectedCash = 100_000_000 - expectedQuantity * expectedPrice;

        var req = new OrderService.OrderReq(expectedCode, expectedQuantity);
        var sut = CreateServiceAsync(expectedCode, expectedPrice, context);

        // act
        await sut.BuyAsync(user.Seq, req, default);

        // assert
        context.ChangeTracker.Clear();

        var cash = await sut.GetCashAsync(user.Seq, default);
        cash.Should().Be(expectedCash);

        var doge = await context.Orders.AsNoTracking().SingleAsync(e => e.UserSeq == user.Seq && e.Code == expectedCode);
        doge.Code.Should().Be(expectedCode);
        doge.Quantity.Should().Be(expectedQuantity);
        doge.Price.Should().Be(expectedPrice);
    }

    [Fact(DisplayName = "코인이 부족하면 팔 수 없다.")]
    public async Task Test4()
    {
        // arrange
        string id = _fixture.IndependentId;
        using var context = _fixture.CreateContext();
        var user = await RegisterAccountAsync(id, context);

        var req = new OrderService.OrderReq("KRW-BTC", 1);
        var sut = CreateServiceAsync("KRW-BTC", 170_000_000, context);

        // act
        Func<Task> action = () => sut.SellAsync(user.Seq, req, default);

        // assert
        await action.Should().ThrowAsync<NotEnoughCoinException>();
    }

    [Fact(DisplayName = "최소 판매 금액을 넘겨야 합니다.")]
    public async Task Test5()
    {
        // arrange
        string id = _fixture.IndependentId;
        using var context = _fixture.CreateContext();
        var user = await RegisterAccountAsync(id, context);

        var buyReq = new OrderService.OrderReq("KRW-DOGE", 110);
        var sellReq = new OrderService.OrderReq("KRW-DOGE", 1);

        var sut = CreateServiceAsync("KRW-DOGE", 280, context);
        await sut.BuyAsync(user.Seq, buyReq, default);

        // act
        Func<Task> action = () => sut.SellAsync(user.Seq, sellReq, default);

        // assert
        await action.Should().ThrowAsync<PriceTooLowException>();
    }

    [Theory(DisplayName = "코인을 판매 한다.")]
    [InlineData("KRW-DOGE", 110.5, 301)]
    public async Task Test6(string code, double quantity, double price)
    {
        // arrange
        string id = _fixture.IndependentId;
        using var context = _fixture.CreateContext();
        var user = await RegisterAccountAsync(id, context);
        double expectedRemainQuantity = 1;

        var buyReq = new OrderService.OrderReq(code, quantity + expectedRemainQuantity);
        var sellReq = new OrderService.OrderReq(code, quantity);
        var expectedCash = 100_000_000 - (buyReq.Quantity - sellReq.Quantity) * price;

        var sut = CreateServiceAsync(code, price, context);
        await sut.BuyAsync(user.Seq, buyReq, default);

        // act
        await sut.SellAsync(user.Seq, sellReq, default);

        // assert
        context.ChangeTracker.Clear();

        var cash = await sut.GetCashAsync(user.Seq, default);
        cash.Should().Be(expectedCash);

        var doges = await context.Orders.AsNoTracking()
            .Where(e => e.UserSeq == user.Seq && e.Code == code)
            .ToListAsync();
        doges.Count.Should().Be(2);
        doges.Sum(e => e.Quantity).Should().Be(expectedRemainQuantity);
    }

    private static async Task<User> RegisterAccountAsync(string id, TradingKingContext context)
    {
        var transaction = new Transaction(context);
        var repo = new UserRepository(context);
        var service = new AccountService(transaction, repo);
        var result = await service.RegisterAsync(id, "xcvjkl;asdfjk;l@@", default);
        Assert.True(result);
        return await context.Users.AsNoTracking().SingleAsync(e => e.Id == id);
    }

    private static OrderService CreateServiceAsync(string code, double price, TradingKingContext context)
    {
        var transaction = new Transaction(context);
        var repo = new OrderRepo(context);

        var mock = new Mock<IExchangeApi>();
        mock.Setup(c => c.GetTickerAsync(code, default))
            .ReturnsAsync([new IExchangeApi.TickerRes(code, price)]);

        return new OrderService(transaction, repo, mock.Object);
    }
}
