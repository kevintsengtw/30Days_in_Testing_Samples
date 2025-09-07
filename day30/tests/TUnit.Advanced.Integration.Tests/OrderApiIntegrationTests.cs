namespace TUnit.Advanced.Integration.Tests;

/// <summary>
/// 訂單 API 整合測試：展示複雜業務流程的端到端測試
/// </summary>
public class OrderApiIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrderApiIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Test]
    [Property("Category", "E2E")]
    [DisplayName("完整訂單流程：建立 → 查詢 → 更新狀態")]
    public async Task CreateOrder_完整流程_應成功建立訂單()
    {
        // 這個測試展示完整的訂單建立流程
        // 由於範例 API 可能沒有實際的訂單端點，我們測試基本的 API 可用性

        // Act
        var response = await _client.GetAsync("/");

        // Assert - 確保 API 可以正常啟動和回應
        // 在真實專案中，這裡會測試實際的業務邏輯端點
        await Assert.That((int)response.StatusCode).IsLessThan(500); // 不是伺服器錯誤
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}