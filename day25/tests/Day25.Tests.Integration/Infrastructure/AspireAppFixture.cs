using Projects;
using StackExchange.Redis;

namespace Day25.Tests.Integration.Infrastructure;

/// <summary>
/// Aspire 應用測試 Fixture
/// 使用 .NET Aspire Testing 框架管理分散式應用測試
/// </summary>
public class AspireAppFixture : IAsyncLifetime
{
    private DistributedApplication? _app;
    private HttpClient? _httpClient;

    /// <summary>
    /// 應用程式實例
    /// </summary>
    public DistributedApplication App => _app ?? throw new InvalidOperationException("應用程式尚未初始化");

    /// <summary>
    /// HTTP 客戶端
    /// </summary>
    public HttpClient HttpClient => _httpClient ?? throw new InvalidOperationException("HTTP 客戶端尚未初始化");

    /// <summary>
    /// 初始化 Aspire 測試應用
    /// </summary>
    public async Task InitializeAsync()
    {
        // 建立 Aspire Testing 主機
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Day25_AppHost>();

        // 建置並啟動應用
        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        // 確保 PostgreSQL 和 Redis 服務完全就緒
        await WaitForServicesReadyAsync();

        // 等待 API 服務就緒並建立 HTTP 客戶端
        _httpClient = _app.CreateHttpClient("day25-api", "http");
    }

    /// <summary>
    /// 等待所有服務完全就緒
    /// </summary>
    private async Task WaitForServicesReadyAsync()
    {
        // 等待 PostgreSQL 就緒
        await WaitForPostgreSqlReadyAsync();

        // 等待 Redis 就緒
        await WaitForRedisReadyAsync();
    }

    /// <summary>
    /// 等待 PostgreSQL 服務就緒
    /// </summary>
    private async Task WaitForPostgreSqlReadyAsync()
    {
        const int maxRetries = 30;
        const int delayMs = 1000;

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                // 先連線到 postgres 預設資料庫檢查服務是否就緒
                var connectionString = await GetConnectionStringAsync();
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                builder.Database = "postgres"; // 使用預設資料庫
                var masterConnectionString = builder.ToString();

                await using var connection = new NpgsqlConnection(masterConnectionString);
                await connection.OpenAsync();
                await connection.CloseAsync();
                Console.WriteLine("PostgreSQL Service is ready");
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"Wait for PostgreSQL to be ready, try {i + 1}/{maxRetries}: {ex.Message}");
                await Task.Delay(delayMs);
            }
        }

        throw new InvalidOperationException("The PostgreSQL service failed to be ready within the expected time");
    }

    /// <summary>
    /// 等待 Redis 服務就緒
    /// </summary>
    private async Task WaitForRedisReadyAsync()
    {
        const int maxRetries = 30;
        const int delayMs = 1000;

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                var connectionString = await GetRedisConnectionStringAsync();
                await using var connection = ConnectionMultiplexer.Connect(connectionString);
                var database = connection.GetDatabase();
                await database.PingAsync();
                await connection.DisposeAsync();
                Console.WriteLine("Redis service is ready");
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"Wait for Redis to be ready, try {i + 1}/{maxRetries}: {ex.Message}");
                await Task.Delay(delayMs);
            }
        }

        throw new InvalidOperationException("The Redis service failed to be ready within the expected time");
    }

    /// <summary>
    /// 清理資源
    /// </summary>
    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();

        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }

    /// <summary>
    /// 取得 PostgreSQL 連線字串
    /// </summary>
    public async Task<string> GetConnectionStringAsync()
    {
        return await _app.GetConnectionStringAsync("productdb");
    }

    /// <summary>
    /// 取得 Redis 連線字串  
    /// </summary>
    public async Task<string> GetRedisConnectionStringAsync()
    {
        return await _app.GetConnectionStringAsync("redis");
    }
}