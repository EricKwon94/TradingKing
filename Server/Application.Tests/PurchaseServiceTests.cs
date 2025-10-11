namespace Application.Tests;

internal class PurchaseServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public PurchaseServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }
}
