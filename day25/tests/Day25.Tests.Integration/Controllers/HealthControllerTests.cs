namespace Day25.Tests.Integration.Controllers;

/// <summary>
/// HealthController 整合測試 - 使用 Aspire Testing
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class HealthControllerTests : IntegrationTestBase
{
    public HealthControllerTests(AspireAppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetHealth_應回傳200與健康狀態()
    {
        // Act
        var response = await HttpClient.GetAsync("/health");

        // Assert
        response.Should().Be200Ok();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }

    [Fact]
    public async Task GetAlive_應回傳200與存活狀態()
    {
        // Act
        var response = await HttpClient.GetAsync("/health/alive");

        // Assert
        response.Should().Be200Ok();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("Healthy");
    }
}