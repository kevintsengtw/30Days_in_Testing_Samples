using Day19.WebApplication.Controllers.Examples.Level1;

namespace Day19.WebApplication.Integration.Tests.Examples.Level1;

/// <summary>
/// Level 1 整合測試：簡單 WebApi 測試
/// 測試重點：路由、HTTP 動詞、模型綁定、狀態碼、回應格式
/// 特色：無資料庫、無外部服務依賴，專注於 Web 層測試
/// </summary>
public class BasicApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicApiControllerTests(WebApplicationFactory<Program> factory)
    {
        this._factory = factory;
    }

    [Fact]
    public async Task Health_無參數_應回傳健康狀態()
    {
        // Arrange
        using var client = this._factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/basicapi/health");

        // Assert
        response.Should().Be200Ok();

        // 驗證 Content-Type 標頭（手動檢查，因為 AwesomeAssertions.Web 不支援 HaveContentType）
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        // 注意：這個測試回傳的是原始 JSON 而不是特定的 DTO，所以需要手動驗證
        var content = await response.Content.ReadAsStringAsync();
        var healthStatus = JsonSerializer.Deserialize<JsonElement>(content);

        healthStatus.GetProperty("status").GetString().Should().Be("Healthy");
        healthStatus.GetProperty("timestamp").GetDateTime().Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Echo_有效訊息_應回傳回音回應()
    {
        // Arrange
        using var client = this._factory.CreateClient();
        var request = new EchoRequest { Message = "Hello World" };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/basicapi/echo", request);

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<EchoResponse>(echoResponse =>
                {
                    echoResponse.OriginalMessage.Should().Be("Hello World");
                    echoResponse.EchoMessage.Should().Be("Echo: Hello World");
                    echoResponse.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
                });
    }

    [Fact]
    public async Task Add_有效數字_應回傳計算結果()
    {
        // Arrange
        using var client = this._factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/basicapi/calculate/add?a=5&b=3");

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<CalculationResult>(calculation =>
                {
                    calculation.Operation.Should().Be("Addition");
                    calculation.Input1.Should().Be(5);
                    calculation.Input2.Should().Be(3);
                    calculation.Result.Should().Be(8);
                    calculation.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
                });
    }
}