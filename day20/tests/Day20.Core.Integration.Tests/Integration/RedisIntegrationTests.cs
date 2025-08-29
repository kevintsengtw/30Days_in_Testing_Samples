namespace Day20.Core.Integration.Tests.Integration;

public class RedisIntegrationTests : IAsyncLifetime
{
    private readonly RedisContainer _redis;
    private IConnectionMultiplexer _connection = null!;
    private IDatabase _database = null!;

    /// <summary>
    /// 建構式，初始化 Redis 容器
    /// </summary>
    public RedisIntegrationTests()
    {
        _redis = new RedisBuilder()
                 .WithImage("redis:7-alpine")
                 .WithCleanUp(true)
                 .Build();
    }

    /// <summary>
    /// 初始化測試環境
    /// </summary>
    public async Task InitializeAsync()
    {
        await _redis.StartAsync();
        _connection = await ConnectionMultiplexer.ConnectAsync(_redis.GetConnectionString());
        _database = _connection.GetDatabase();
    }

    /// <summary>
    /// 清理測試環境
    /// </summary>
    public async Task DisposeAsync()
    {
        _connection?.Dispose();
        await _redis.DisposeAsync();
    }

    [Fact]
    public async Task StringSetAsync_設定字串值_應正確儲存和讀取()
    {
        // Arrange
        const string key = "test:user:123";
        const string value = "測試使用者資料";

        // Act
        await _database.StringSetAsync(key, value);
        var retrievedValue = await _database.StringGetAsync(key);

        // Assert
        retrievedValue.Should().Be(value);
    }

    [Fact]
    public async Task StringSetAsync_設定過期時間_應能正確設定TTL()
    {
        // Arrange
        const string key = "test:session:456";
        const string value = "session_data";
        var expiry = TimeSpan.FromSeconds(10); // 使用較長的過期時間

        // Act
        await _database.StringSetAsync(key, value, expiry);
        var immediateValue = await _database.StringGetAsync(key);
        var ttl = await _database.KeyTimeToLiveAsync(key);

        // Assert
        immediateValue.Should().Be(value);
        ttl.Should().NotBeNull();
        ttl.Value.TotalSeconds.Should().BeGreaterThan(0);
        ttl.Value.TotalSeconds.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public async Task HashSetAsync_設定雜湊結構_應正確操作多個欄位()
    {
        // Arrange
        const string key = "user:profile:789";
        var userHash = new HashEntry[]
        {
            new("name", "張三"),
            new("email", "zhang@example.com"),
            new("age", "30")
        };

        // Act
        await _database.HashSetAsync(key, userHash);
        var name = await _database.HashGetAsync(key, "name");
        var email = await _database.HashGetAsync(key, "email");
        var age = await _database.HashGetAsync(key, "age");

        // Assert
        name.Should().Be("張三");
        email.Should().Be("zhang@example.com");
        age.Should().Be("30");
    }

    [Fact]
    public async Task ListPushAsync_操作列表結構_應正確維護順序()
    {
        // Arrange
        const string key = "user:notifications:101";
        var notifications = new[] { "通知1", "通知2", "通知3" };

        // Act
        foreach (var notification in notifications)
        {
            await _database.ListRightPushAsync(key, notification);
        }

        var listLength = await _database.ListLengthAsync(key);
        var firstItem = await _database.ListGetByIndexAsync(key, 0);
        var lastItem = await _database.ListGetByIndexAsync(key, -1);

        // Assert
        listLength.Should().Be(3);
        firstItem.Should().Be("通知1");
        lastItem.Should().Be("通知3");
    }

    [Fact]
    public async Task SetAddAsync_操作集合結構_應正確去重和查詢成員()
    {
        // Arrange
        const string key = "user:tags:202";
        var tags = new[] { "開發者", "測試", "開發者", "自動化" };

        // Act
        foreach (var tag in tags)
        {
            await _database.SetAddAsync(key, tag);
        }

        var setSize = await _database.SetLengthAsync(key);
        var isMember = await _database.SetContainsAsync(key, "開發者");
        var isNotMember = await _database.SetContainsAsync(key, "不存在的標籤");

        // Assert
        setSize.Should().Be(3); // 自動去重
        isMember.Should().BeTrue();
        isNotMember.Should().BeFalse();
    }

    [Fact]
    public async Task KeyExistsAsync_檢查鍵值存在_應正確回傳狀態()
    {
        // Arrange
        const string existingKey = "test:exists:key";
        const string nonExistingKey = "test:not:exists";

        // Act
        await _database.StringSetAsync(existingKey, "value");
        var exists = await _database.KeyExistsAsync(existingKey);
        var notExists = await _database.KeyExistsAsync(nonExistingKey);

        // Assert
        exists.Should().BeTrue();
        notExists.Should().BeFalse();
    }

    [Fact]
    public async Task KeyDeleteAsync_刪除鍵值_應成功移除資料()
    {
        // Arrange
        const string key = "test:delete:key";
        const string value = "to be deleted";

        // Act
        await _database.StringSetAsync(key, value);
        var existsBeforeDelete = await _database.KeyExistsAsync(key);

        var deleteResult = await _database.KeyDeleteAsync(key);
        var existsAfterDelete = await _database.KeyExistsAsync(key);

        // Assert
        existsBeforeDelete.Should().BeTrue();
        deleteResult.Should().BeTrue();
        existsAfterDelete.Should().BeFalse();
    }
}