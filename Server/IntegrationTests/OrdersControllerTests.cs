using Application.Orchestrations;
using FluentAssertions;
using Host;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationTests;

public class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public OrdersControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact(DisplayName = "코인을 구매한다.")]
    public async Task Test1()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        await _factory.RegisterAsync(_client, id, pwd, true);

        var content = new OrderService.OrderReq("KRW-DOGE", 110.5).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/orders/buy", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "최소 주문 금액을 넘겨야 한다.")]
    public async Task Test2()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        await _factory.RegisterAsync(_client, id, pwd, true);

        var content = new OrderService.OrderReq("KRW-DOGE", 1).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/orders/buy", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "돈이 없으면 코인을 구매할 수 없다.")]
    public async Task Test3()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        await _factory.RegisterAsync(_client, id, pwd, true);

        var content = new OrderService.OrderReq("KRW-BTC", 1.2).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/orders/buy", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact(DisplayName = "로그인을 해야한다.")]
    public async Task Test4()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        await _factory.RegisterAsync(_client, id, pwd, false);

        var content = new OrderService.OrderReq("KRW-DOGE", 110.5).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/orders/buy", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
