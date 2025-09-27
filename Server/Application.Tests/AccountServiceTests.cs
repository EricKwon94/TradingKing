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
        string id = "string1";
        string pwd = "asdasd";
        string expectedPwd = new Encryptor().Encrypt(pwd);
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();
        var transaction = new Transaction(context);
        var repo = new UserRepository(context);

        var sut = new AccountService(transaction, repo);

        // act
        bool result = await sut.RegisterAsync(id, pwd, default);

        // assert
        result.Should().BeTrue();
        var user = await context.Users.FirstAsync(e => e.Id == id);
        user.Id.Should().Be(id);
        user.Password.Should().Be(expectedPwd);
        user.Jwt.Should().BeNull();
    }

    [Fact]
    public async Task Can_not_register_user_if_already_exist()
    {
        // arrange
        string id = "string1";
        string pwd = "asdasd";
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();
        var transaction = new Transaction(context);
        var repo = new UserRepository(context);

        var sut = new AccountService(transaction, repo);
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
        string id = "string1";
        string pwd = "asdasd";
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();
        var transaction = new Transaction(context);
        var repo = new UserRepository(context); ;

        var sut = new AccountService(transaction, repo);

        bool r = await sut.RegisterAsync(id, pwd, default);
        Assert.True(r);
        context.ChangeTracker.Clear();

        // act
        string? expectedJwt = await sut.LoginAsync(id, pwd, ISSKEY, "iss", "aud", default);

        // assert
        expectedJwt.Should().NotBeNullOrEmpty();
        var user = await context.Users.FirstAsync(e => e.Id == id);
        user.Jwt.Should().Be(expectedJwt);
    }
}