using System.Diagnostics;
using Day23.Integration.Tests.Infrastructure;

namespace Day23.Integration.Tests.Controllers;

/// <summary>
/// 健康檢查控制器測試
/// </summary>
public class HealthControllerTests : IntegrationTestBase
{
    public HealthControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Get_Health_應回傳200狀態碼()
    {
        // Arrange
        // (無需特別準備)

        // Act
        var response = await HttpClient.GetAsync("/health");

        // Assert
        response.Should().Be200Ok();
    }

    [Fact]
    public async Task Get_Health_應回傳正確的回應內容()
    {
        // Arrange
        // (無需特別準備)

        // Act
        var response = await HttpClient.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.Should().Be200Ok();
        content.Should().Contain("\"status\":\"ok\"");
        content.Should().Contain("\"timestamp\":");
        content.Should().Contain("\"version\":\"1.0.0\"");
    }

    [Fact]
    public async Task Get_Health_應能快速回應()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await HttpClient.GetAsync("/health");

        stopwatch.Stop();

        // Assert
        response.Should().Be200Ok();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 應在 1 秒內回應
    }
}