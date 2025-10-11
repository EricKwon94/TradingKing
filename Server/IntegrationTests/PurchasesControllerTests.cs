using Application.Orchestrations;
using FluentAssertions;
using Host;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationTests;

public class PurchasesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public PurchasesControllerTests(CustomWebApplicationFactory<Program> factory)
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
        var user = await _factory.RegisterAsync(_client, id, pwd);

        var content = new PurchaseService.PurchaseReq(user.Seq, "KRW-DOGE", 11.5, 301).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/purchases/buy", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "돈이 없으면 코인을 구매할 수 없다.")]
    public async Task Test2()
    {
        // arrange
        string id = _factory.IndependentId;
        string pwd = "asdasd";
        var user = await _factory.RegisterAsync(_client, id, pwd);

        var content = new PurchaseService.PurchaseReq(user.Seq, "KRW-BTC", 1.2, 175_000_000).ToContent();

        // act
        HttpResponseMessage res = await _client.PostAsync("/purchases/buy", content);

        // assert
        res.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
