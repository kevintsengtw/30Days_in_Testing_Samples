using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Time.Testing;

namespace Day23.Integration.Tests.Infrastructure;

/// <summary>
/// 整合測試的 WebApplicationFactory 基底類別
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    private RedisContainer? _redisContainer;
    private FakeTimeProvider? _timeProvider;

    public PostgreSqlContainer PostgresContainer => _postgresContainer
                                                    ?? throw new InvalidOperationException("PostgreSQL container 尚未初始化");

    public RedisContainer RedisContainer => _redisContainer
                                            ?? throw new InvalidOperationException("Redis container 尚未初始化");

    public FakeTimeProvider TimeProvider => _timeProvider
                                            ?? throw new InvalidOperationException("TimeProvider 尚未初始化");

    public async Task InitializeAsync()
    {
        // 建立 PostgreSQL container
        _postgresContainer = new PostgreSqlBuilder()
                             .WithImage("postgres:16-alpine")
                             .WithDatabase("day23_test")
                             .WithUsername("testuser")
                             .WithPassword("testpass")
                             .WithCleanUp(true)
                             .Build();

        // 建立 Redis container
        _redisContainer = new RedisBuilder()
                          .WithImage("redis:7-alpine")
                          .WithCleanUp(true)
                          .Build();

        // 建立 FakeTimeProvider
        _timeProvider = new FakeTimeProvider(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // 啟動容器
        await _postgresContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            // 移除現有的設定來源
            config.Sources.Clear();

            // 添加測試專用設定
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = PostgresContainer.GetConnectionString(),
                ["ConnectionStrings:Redis"] = RedisContainer.GetConnectionString(),
                ["Logging:LogLevel:Default"] = "Warning",
                ["Logging:LogLevel:System"] = "Warning",
                ["Logging:LogLevel:Microsoft"] = "Warning"
            });
        });

        builder.ConfigureServices(services =>
        {
            // 替換 TimeProvider 為 FakeTimeProvider
            services.Remove(services.Single(d => d.ServiceType == typeof(TimeProvider)));
            services.AddSingleton<TimeProvider>(TimeProvider);
        });

        builder.UseEnvironment("Testing");
    }

    public new async Task DisposeAsync()
    {
        if (_postgresContainer != null)
        {
            await _postgresContainer.DisposeAsync();
        }

        if (_redisContainer != null)
        {
            await _redisContainer.DisposeAsync();
        }

        await base.DisposeAsync();
    }
}