using Application.Orchestrations;
using FluentAssertions;
using Host;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTests;

public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AccountControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Register_user()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        var content = new AccountService.RegisterReq(id, pwd).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/account/register", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Can_not_register_user_if_already_exist()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        var content = new AccountService.RegisterReq(id, pwd).ToContent();
        await RegisterAsync(id, pwd, default);

        // act
        HttpResponseMessage res = await _client.PostAsync("/account/register", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Can_not_register_if_invalid_id()
    {
        // arrange
        string id = "asda^;";
        string pwd = "asdasd";
        var content = new AccountService.RegisterReq(id, pwd).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/account/register", content);

        // assert
        string body = await res.Content.ReadAsStringAsync();
        System.Console.WriteLine(body);
        body.Should().Be("-1");
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Can_not_register_if_invalid_pwd()
    {
        // arrange
        string id = "asda123";
        string pwd = "asd";
        var content = new AccountService.RegisterReq(id, pwd).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/account/register", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_if_user_exist()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        await RegisterAsync(id, pwd, default);
        var content = new AccountService.LoginReq(id, pwd).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/account/login", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);
        string body = await res.Content.ReadAsStringAsync();
        body.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Can_not_login_if_user_no_exist()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        var content = new AccountService.LoginReq(id, pwd).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/account/login", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task RegisterAsync(string id, string pwd, CancellationToken ct)
    {
        var content = new AccountService.RegisterReq(id, pwd).ToContent();
        var res = await _client.PostAsync("/account/register", content, ct);
        res.EnsureSuccessStatusCode();
    }
}
