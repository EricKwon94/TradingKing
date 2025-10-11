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
using System.Threading.Tasks;

namespace Application.Tests;

public class PurchaseServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public PurchaseServiceTests(TestDatabaseFixture fixture)
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

        var req = new PurchaseService.PurchaseReq("KRW-BTC", 1);
        var sut = CreateServiceAsync("KRW-BTC", 170_000_000, context);

        // act
        Func<Task> action = () => sut.BuyAsync(user.Seq, req, default);

        // assert
        await action.Should().ThrowAsync<NotEnoughCashException>();
    }

    [Theory(DisplayName = "코인을 구매 한다.")]
    [InlineData("KRW-DOGE", 11.5, 301)]
    public async Task Test2(string expectedCode, double expectedQuantity, double expectedPrice)
    {
        // arrange
        string id = _fixture.IndependentId;
        using var context = _fixture.CreateContext();
        var user = await RegisterAccountAsync(id, context);
        var expectedCash = 100_000_000 - expectedQuantity * expectedPrice;

        var req = new PurchaseService.PurchaseReq(expectedCode, expectedQuantity);
        var sut = CreateServiceAsync(expectedCode, expectedPrice, context);

        // act
        await sut.BuyAsync(user.Seq, req, default);

        // assert
        context.ChangeTracker.Clear();

        var cash = await sut.GetCashAsync(user.Seq, default);
        cash.Should().Be(expectedCash);

        var doge = await context.Purchases.AsNoTracking().SingleAsync(e => e.Code == expectedCode);
        doge.Code.Should().Be(expectedCode);
        doge.Quantity.Should().Be(expectedQuantity);
        doge.Price.Should().Be(expectedPrice);
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

    private static PurchaseService CreateServiceAsync(string code, double price, TradingKingContext context)
    {
        var transaction = new Transaction(context);
        var repo = new PurchaseRepo(context);

        var mock = new Mock<IExchangeApi>();
        mock.Setup(c => c.GetTickerAsync(code, default))
            .ReturnsAsync([new IExchangeApi.TickerRes(code, price)]);

        return new PurchaseService(transaction, repo, mock.Object);
    }
}
