using Application.Gateways;
using Application.Orchestrations;
using Domain;
using FluentAssertions;
using Infrastructure.EFCore;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using Moq;
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
        var sut = CreateService(expectedCode, expectedPrice, context);

        // act
        await sut.BuyAsync(user.Id, req, default);

        // assert
        context.ChangeTracker.Clear();

        var cashes = await context.Orders.AsNoTracking()
            .Where(e => e.UserId == user.Id && e.Code == "KRW-CASH")
            .ToListAsync();
        cashes.Sum(e => e.Quantity).Should().Be(expectedCash);

        var doge = await context.Orders.AsNoTracking().SingleAsync(e => e.UserId == user.Id && e.Code == expectedCode);
        doge.SeasonId.Should().Be(_fixture.ExpectedLastSeasonId);
        doge.Code.Should().Be(expectedCode);
        doge.Quantity.Should().Be(expectedQuantity);
        doge.Price.Should().Be(expectedPrice);
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

        var sut = CreateService(code, price, context);
        await sut.BuyAsync(user.Id, buyReq, default);
        context.ChangeTracker.Clear();

        // act
        await sut.SellAsync(user.Id, sellReq, default);

        // assert
        context.ChangeTracker.Clear();

        var cashes = await context.Orders.AsNoTracking()
            .Where(e => e.UserId == user.Id && e.Code == "KRW-CASH")
            .ToListAsync();
        cashes.Sum(e => e.Quantity).Should().Be(expectedCash);

        var doges = await context.Orders.AsNoTracking()
            .Where(e => e.UserId == user.Id && e.Code == code)
            .ToListAsync();
        doges.Count.Should().Be(2);
        doges.Sum(e => e.Quantity).Should().Be(expectedRemainQuantity);
    }

    private static async Task<User> RegisterAccountAsync(string id, TradingKingContext context)
    {
        var transaction = new Transaction(context);
        var repo = new UserRepository(context);
        var seasonRepo = new SeasonRepo(context);
        var service = new AccountService(transaction, repo, seasonRepo);
        var result = await service.RegisterAsync(id, "xcvjkl;asdfjk;l@@", default);
        Assert.True(result);
        context.ChangeTracker.Clear();
        return await context.Users.AsNoTracking().SingleAsync(e => e.Id == id);
    }

    private static OrderService CreateService(string code, double price, TradingKingContext context)
    {
        var transaction = new Transaction(context);
        var repo = new OrderRepo(context);
        var userRepo = new UserRepository(context);
        var seasonRepo = new SeasonRepo(context);

        var mock = new Mock<IExchangeApi>();
        mock.Setup(c => c.GetTickerAsync(code, default))
            .ReturnsAsync([new IExchangeApi.TickerRes(code, price)]);

        return new OrderService(transaction, repo, userRepo, seasonRepo, mock.Object);
    }
}
