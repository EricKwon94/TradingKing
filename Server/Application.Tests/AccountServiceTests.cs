using Application.Orchestrations;
using Common;
using FluentAssertions;
using Infrastructure.EFCore;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Application.Tests;

public class AccountServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;
    private const string ISSKEY = "x08z5dZcVX4QDJa3!QumT8?5y1Ezzp2bXjzHeDgzfR";

    public AccountServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Register_user()
    {
        // arrange
        string id = _fixture.IndependentId;
        string pwd = "asdasd";
        string expectedPwd = new Encryptor().Encrypt(pwd);
        using var context = _fixture.CreateContext();

        var sut = CreateAccountService(context);

        // act
        bool result = await sut.RegisterAsync(id, pwd, default);

        // assert
        result.Should().BeTrue();
        var user = await context.Users.AsNoTracking().SingleAsync(e => e.Id == id);
        user.Id.Should().Be(id);
        user.Password.Should().Be(expectedPwd);
        user.Jwt.Should().BeNull();

        var order = await context.Orders.AsNoTracking().SingleAsync(e => e.UserSeq == user.Seq);
        order.Code.Should().Be("KRW-CASH");
        order.Price.Should().Be(100_000_000);
        order.Quantity.Should().Be(1);
    }

    [Fact]
    public async Task Can_not_register_user_if_already_exist()
    {
        // arrange
        string id = _fixture.IndependentId;
        string pwd = "asdasd";
        using var context = _fixture.CreateContext();

        var sut = CreateAccountService(context);
        bool r = await sut.RegisterAsync(id, pwd, default);
        Assert.True(r);

        // act
        bool result = await sut.RegisterAsync(id, pwd, default);

        // assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Can_login_if_user_exist()
    {
        // arrange
        string id = _fixture.IndependentId;
        string pwd = "asdasd";
        using var context = _fixture.CreateContext();
        var sut = CreateAccountService(context);

        bool r = await sut.RegisterAsync(id, pwd, default);
        Assert.True(r);
        context.ChangeTracker.Clear();

        // act
        string? expectedJwt = await sut.LoginAsync(id, pwd, ISSKEY, "iss", "aud", default);

        // assert
        expectedJwt.Should().NotBeNullOrEmpty();
        var user = await context.Users.SingleAsync(e => e.Id == id);
        user.Jwt.Should().Be(expectedJwt);
    }

    [Fact]
    public async Task Can_not_login_if_user_no_exist()
    {
        // arrange
        string id = _fixture.IndependentId;
        string pwd = "asdasd";
        using var context = _fixture.CreateContext();
        var sut = CreateAccountService(context);

        // act
        string? jwt = await sut.LoginAsync(id, pwd, ISSKEY, "iss", "aud", default);

        // assert
        jwt.Should().BeNullOrEmpty();
        var user = await context.Users.SingleOrDefaultAsync(e => e.Id == id);
        user.Should().BeNull();
    }

    private AccountService CreateAccountService(TradingKingContext context)
    {
        var transaction = new Transaction(context);
        var repo = new UserRepository(context);
        return new AccountService(transaction, repo);
    }
}