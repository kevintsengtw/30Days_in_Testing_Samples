using Projects;
using StackExchange.Redis;

namespace Day25.Tests.Integration;

/// <summary>
/// 驗證 Aspire 容器使用情況的簡單測試
/// </summary>
public class VerifyAspireContainers : IAsyncLifetime
{
    private DistributedApplication? _app;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Day25_AppHost>();

        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        // 等待 PostgreSQL 和 Redis 服務完全就緒
        await WaitForPostgreSqlReadyAsync();
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
                var connectionString = await _app!.GetConnectionStringAsync("productdb");
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                builder.Database = "postgres"; // 使用預設資料庫
                var masterConnectionString = builder.ToString();

                await using var connection = new NpgsqlConnection(masterConnectionString);
                await connection.OpenAsync();
                await connection.CloseAsync();
                Console.WriteLine("PostgreSQL 服務已就緒");
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"等待 PostgreSQL 就緒，嘗試 {i + 1}/{maxRetries}: {ex.Message}");
                await Task.Delay(delayMs);
            }
        }

        throw new InvalidOperationException("PostgreSQL 服務未能在預期時間內就緒");
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
                var connectionString = await _app!.GetConnectionStringAsync("redis");
                await using var connection = ConnectionMultiplexer.Connect(connectionString);
                var database = connection.GetDatabase();
                await database.PingAsync();
                await connection.DisposeAsync();
                Console.WriteLine("Redis 服務已就緒");
                return;
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                Console.WriteLine($"等待 Redis 就緒，嘗試 {i + 1}/{maxRetries}: {ex.Message}");
                await Task.Delay(delayMs);
            }
        }

        throw new InvalidOperationException("Redis 服務未能在預期時間內就緒");
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }

    [Fact]
    public async Task 驗證_Aspire_容器連線字串()
    {
        // Arrange & Act
        var postgresConnectionString = await _app!.GetConnectionStringAsync("productdb");
        var redisConnectionString = await _app!.GetConnectionStringAsync("redis");

        // Assert
        Console.WriteLine($"PostgreSQL 連線字串: {postgresConnectionString}");
        Console.WriteLine($"Redis 連線字串: {redisConnectionString}");

        // 驗證 PostgreSQL 連線字串格式和動態埠
        postgresConnectionString.Should().Contain("Host=localhost");
        postgresConnectionString.Should().Contain("Port=");
        postgresConnectionString.Should().NotContain("Port=5432"); // 不是預設埠

        // 驗證 Redis 連線字串格式和動態埠
        redisConnectionString.Should().Contain("localhost:");
        redisConnectionString.Should().NotContain(":6379"); // 不是預設埠
    }

    [Fact]
    public async Task 驗證_可以實際連線到PostgreSQL()
    {
        // Arrange
        var connectionString = await _app!.GetConnectionStringAsync("productdb");

        // 首先連到 postgres 預設資料庫
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        builder.Database = "postgres";
        var masterConnectionString = builder.ToString();

        // Act & Assert
        await using var connection = new NpgsqlConnection(masterConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT version()";
        var version = await command.ExecuteScalarAsync();

        Console.WriteLine($"PostgreSQL 版本: {version}");
        version.Should().NotBeNull();
        version.ToString().Should().Contain("PostgreSQL");
    }
}