namespace Day23.Integration.Tests.Infrastructure;

/// <summary>
/// 整合測試基底類別 - 使用 Collection Fixture 共享容器
/// </summary>
[Collection("Integration Tests")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient HttpClient;
    protected readonly DatabaseManager DatabaseManager;
    protected readonly IFlurlClient FlurlClient;

    protected IntegrationTestBase(TestWebApplicationFactory factory)
    {
        Factory = factory;
        HttpClient = factory.CreateClient();
        DatabaseManager = new DatabaseManager(factory.PostgresContainer.GetConnectionString());

        // 設定 Flurl 用戶端
        FlurlClient = new FlurlClient(HttpClient);
    }

    public virtual async Task InitializeAsync()
    {
        // 初始化資料庫結構
        await DatabaseManager.InitializeDatabaseAsync();
    }

    public virtual async Task DisposeAsync()
    {
        // 清理資料庫資料
        await DatabaseManager.CleanDatabaseAsync();

        FlurlClient.Dispose();
    }

    /// <summary>
    /// 重設時間為測試開始時間
    /// </summary>
    protected void ResetTime()
    {
        Factory.TimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
    }

    /// <summary>
    /// 前進時間
    /// </summary>
    protected void AdvanceTime(TimeSpan timeSpan)
    {
        Factory.TimeProvider.Advance(timeSpan);
    }

    /// <summary>
    /// 設定特定時間
    /// </summary>
    protected void SetTime(DateTimeOffset time)
    {
        Factory.TimeProvider.SetUtcNow(time);
    }
}