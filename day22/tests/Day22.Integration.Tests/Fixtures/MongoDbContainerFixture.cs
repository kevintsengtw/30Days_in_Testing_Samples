namespace Day22.Integration.Tests.Fixtures;

/// <summary>
/// MongoDB 容器 Fixture
/// </summary>
public class MongoDbContainerFixture : IAsyncLifetime
{
    private MongoDbContainer? _container;

    /// <summary>
    /// MongoDB 資料庫實例
    /// </summary>
    public IMongoDatabase Database { get; private set; } = null!;

    /// <summary>
    /// MongoDB 連線字串
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string DatabaseName { get; } = "testdb";

    /// <summary>
    /// 初始化容器
    /// </summary>
    public async Task InitializeAsync()
    {
        _container = new MongoDbBuilder()
                     .WithImage("mongo:7.0")
                     .WithPortBinding(27017, true)
                     .Build();

        await _container.StartAsync();

        ConnectionString = _container.GetConnectionString();
        var client = new MongoClient(ConnectionString);
        Database = client.GetDatabase(DatabaseName);
    }

    /// <summary>
    /// 清理容器資源
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }

    /// <summary>
    /// 清空所有集合
    /// </summary>
    public async Task ClearDatabaseAsync()
    {
        var collections = await Database.ListCollectionNamesAsync();
        await collections.ForEachAsync(async collectionName => { await Database.DropCollectionAsync(collectionName); });
    }
}

/// <summary>
/// MongoDB Collection Fixture 定義
/// </summary>
[CollectionDefinition("MongoDb Collection")]
public class MongoDbCollectionFixture : ICollectionFixture<MongoDbContainerFixture>
{
}