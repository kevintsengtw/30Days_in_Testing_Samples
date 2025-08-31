namespace Day22.Integration.Tests.Fixtures;

/// <summary>
/// Redis 容器 Fixture
/// </summary>
public class RedisContainerFixture : IAsyncLifetime
{
    private RedisContainer? _container;

    /// <summary>
    /// Redis 連線物件
    /// </summary>
    public IConnectionMultiplexer Connection { get; private set; } = null!;

    /// <summary>
    /// Redis 資料庫實例
    /// </summary>
    public IDatabase Database { get; private set; } = null!;

    /// <summary>
    /// Redis 連線字串
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// 初始化容器
    /// </summary>
    public async Task InitializeAsync()
    {
        _container = new RedisBuilder()
                     .WithImage("redis:7.2")
                     .WithPortBinding(6379, true)
                     .Build();

        await _container.StartAsync();

        ConnectionString = _container.GetConnectionString();
        Connection = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
        Database = Connection.GetDatabase();
    }

    /// <summary>
    /// 清理容器資源
    /// </summary>
    public async Task DisposeAsync()
    {
        if (Connection != null)
        {
            await Connection.DisposeAsync();
        }

        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }

    /// <summary>
    /// 清空 Redis 資料庫
    /// </summary>
    public async Task ClearDatabaseAsync()
    {
        // 使用 DEL 命令逐一刪除 keys，而不是 FLUSHDB
        // 這是因為 Redis 容器可能沒有啟用 admin 模式
        var keys = Connection.GetServer(Connection.GetEndPoints().First()).Keys(Database.Database);
        if (keys.Any())
        {
            await Database.KeyDeleteAsync(keys.ToArray());
        }
    }
}

/// <summary>
/// Redis Collection Fixture 定義
/// </summary>
[CollectionDefinition("Redis Collection")]
public class RedisCollectionFixture : ICollectionFixture<RedisContainerFixture>
{
}