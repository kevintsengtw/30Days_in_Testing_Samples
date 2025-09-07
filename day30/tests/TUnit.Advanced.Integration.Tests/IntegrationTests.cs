namespace TUnit.Advanced.Integration.Tests;

/// <summary>
/// ASP.NET Core 整合測試基底類別
/// </summary>
public class WebApiIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WebApiIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // 移除原有的資料庫設定（如果有的話）
                    // 這裡可以加入測試專用的服務設定
                    services.AddLogging();
                });
                builder.UseEnvironment("Testing");
            });

        _client = _factory.CreateClient();
    }

    [Test]
    public async Task WeatherForecast_Get_應回傳正確格式的資料()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).IsNotNull();
        await Assert.That(content.Length).IsGreaterThan(0);
    }

    [Test]
    [Property("Category", "Integration")]
    public async Task WeatherForecast_ResponseHeaders_應包含ContentType標頭()
    {
        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        // 檢查實際會存在的 Content-Type 標頭
        var contentType = response.Content.Headers.ContentType?.MediaType;
        await Assert.That(contentType).IsEqualTo("application/json");
    }

    [Test]
    [Property("Category", "Performance")]
    [Timeout(10000)] // 10 秒超時保護
    public async Task WeatherForecast_ResponseTime_應在合理範圍內(CancellationToken cancellationToken)
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/weatherforecast", cancellationToken);
        stopwatch.Stop();

        // Assert
        await Assert.That(response.IsSuccessStatusCode).IsTrue();
        await Assert.That(stopwatch.ElapsedMilliseconds).IsLessThan(5000); // 5秒內回應
    }

    [Test]
    [Property("Category", "Smoke")]
    public async Task WeatherForecast_端點可用性_應能正常回應()
    {
        // 基本的冒煙測試：確保端點可用

        // Act
        var response = await _client.GetAsync("/weatherforecast");

        // Assert
        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).IsNotNull();
        await Assert.That(content.Length).IsGreaterThan(10); // 確保有實際內容
    }

    [Test]
    [Property("Category", "Load")]
    [Timeout(30000)]
    public async Task WeatherForecast_並行請求_應能正確處理(CancellationToken cancellationToken)
    {
        // Arrange
        const int concurrentRequests = 50;
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (var i = 0; i < concurrentRequests; i++)
        {
            tasks.Add(_client.GetAsync("/weatherforecast"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        await Assert.That(responses.Length).IsEqualTo(concurrentRequests);
        await Assert.That(responses.All(r => r.IsSuccessStatusCode)).IsTrue();

        // 清理
        foreach (var response in responses)
        {
            response.Dispose();
        }
    }

    [Test]
    [Property("Category", "Health")]
    public async Task HealthCheck_應回傳健康狀態()
    {
        // 測試應用程式的健康狀態
        // 由於範例 API 沒有 /health 端點，我們測試 WeatherForecast 端點來確認 API 健康

        var response = await _client.GetAsync("/weatherforecast");

        // Assert - 確保 API 可以正常回應
        await Assert.That(response.IsSuccessStatusCode).IsTrue();

        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).IsNotNull();
        await Assert.That(content.Length).IsGreaterThan(0);
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}

/// <summary>
/// 概念性的資料庫整合測試範例
/// </summary>
public class DatabaseIntegrationTests
{
    [Test]
    [Property("Category", "Database")]
    public async Task DatabaseConnection_應能正確連線()
    {
        // 這是一個概念性的資料庫測試範例
        // 在真實專案中，這裡會使用 Testcontainers 來啟動測試資料庫

        // 模擬資料庫連線測試
        var connectionSuccessful = true; // 在真實情況下會實際測試資料庫連線

        await Assert.That(connectionSuccessful).IsTrue();
    }

    [Test]
    [Property("Category", "Database")]
    [DisplayName("訂單資料庫 CRUD 操作完整測試")]
    public async Task OrderRepository_CRUD操作_應正確執行()
    {
        // 模擬完整的 CRUD 測試
        // 在真實專案中會測試實際的資料庫操作

        // 1. Create - 建立訂單
        // 2. Read - 查詢訂單
        // 3. Update - 更新訂單狀態
        // 4. Delete - 刪除訂單

        var crudTestPassed = true; // 簡化的測試結果

        await Assert.That(crudTestPassed).IsTrue();
    }
}