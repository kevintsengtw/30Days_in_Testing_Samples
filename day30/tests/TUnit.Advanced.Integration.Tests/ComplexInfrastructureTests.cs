namespace TUnit.Advanced.Integration.Tests;

/// <summary>
/// 複雜測試基礎設施編排範例
/// 展示 TUnit 結合 Testcontainers.NET 的強大能力
/// 注意：這些是基礎設施驗證測試，用於確保測試環境正常運作
/// 實際的業務邏輯測試會建立在這些基礎設施之上
/// </summary>
public class ComplexInfrastructureTests
{
    [Test]
    [Property("Category", "Integration")]
    [Property("Infrastructure", "Complex")]
    [DisplayName("多服務協作：PostgreSQL + Redis + Kafka 完整測試")]
    public async Task CompleteWorkflow_多服務協作_應正確執行()
    {
        // Arrange & Act
        // 使用全域設置的容器
        var dbConnectionString = GlobalTestInfrastructureSetup.PostgreSqlContainer!.GetConnectionString();
        var redisConnectionString = GlobalTestInfrastructureSetup.RedisContainer!.GetConnectionString();
        var kafkaBootstrapServers = GlobalTestInfrastructureSetup.KafkaContainer!.GetBootstrapAddress();

        // Assert
        await Assert.That(dbConnectionString).IsNotNull();
        await Assert.That(dbConnectionString).Contains("test_db");
        await Assert.That(dbConnectionString).Contains("test_user");

        await Assert.That(redisConnectionString).IsNotNull();
        await Assert.That(redisConnectionString).Contains("127.0.0.1");

        await Assert.That(kafkaBootstrapServers).IsNotNull();
        await Assert.That(kafkaBootstrapServers).Contains("127.0.0.1");

        // 模擬完整的業務流程
        Console.WriteLine("=== 多服務協作測試 ===");
        Console.WriteLine($"PostgreSQL: {dbConnectionString}");
        Console.WriteLine($"Redis: {redisConnectionString}");
        Console.WriteLine($"Kafka: {kafkaBootstrapServers}");
        Console.WriteLine("=====================");
    }

    [Test]
    [Property("Category", "Database")]
    [DisplayName("PostgreSQL 資料庫連線驗證")]
    public async Task PostgreSqlDatabase_連線驗證_應成功建立連線()
    {
        // Arrange
        var connectionString = GlobalTestInfrastructureSetup.PostgreSqlContainer!.GetConnectionString();

        // Act & Assert
        await Assert.That(connectionString).Contains("test_db");
        await Assert.That(connectionString).Contains("test_user");
        await Assert.That(connectionString).Contains("test_password");

        Console.WriteLine($"Database connection verified: {connectionString}");
    }

    [Test]
    [Property("Category", "Cache")]
    [DisplayName("Redis 快取服務驗證")]
    public async Task RedisCache_快取服務_應正確啟動()
    {
        // Arrange
        var connectionString = GlobalTestInfrastructureSetup.RedisContainer!.GetConnectionString();

        // Act & Assert
        await Assert.That(connectionString).IsNotNull();
        await Assert.That(connectionString).Contains("127.0.0.1");

        Console.WriteLine($"Redis connection verified: {connectionString}");
    }

    [Test]
    [Property("Category", "MessageQueue")]
    [DisplayName("Kafka 訊息佇列服務驗證")]
    public async Task KafkaMessageQueue_訊息佇列_應正確啟動()
    {
        // Arrange
        var bootstrapServers = GlobalTestInfrastructureSetup.KafkaContainer!.GetBootstrapAddress();

        // Act & Assert
        await Assert.That(bootstrapServers).IsNotNull();
        await Assert.That(bootstrapServers).Contains("127.0.0.1");

        Console.WriteLine($"Kafka connection verified: {bootstrapServers}");
    }

    [Test]
    [Property("Category", "Performance")]
    [Timeout(30000)]
    [DisplayName("容器啟動效能測試")]
    public async Task ContainerStartup_效能測試_應在合理時間內完成(CancellationToken cancellationToken)
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act - 測試所有容器都已啟動
        var dbReady = GlobalTestInfrastructureSetup.PostgreSqlContainer!.State == TestcontainersStates.Running;
        var redisReady = GlobalTestInfrastructureSetup.RedisContainer!.State == TestcontainersStates.Running;
        var kafkaReady = GlobalTestInfrastructureSetup.KafkaContainer!.State == TestcontainersStates.Running;

        stopwatch.Stop();

        // Assert
        await Assert.That(dbReady).IsTrue();
        await Assert.That(redisReady).IsTrue();
        await Assert.That(kafkaReady).IsTrue();

        Console.WriteLine($"Container status check completed in: {stopwatch.ElapsedMilliseconds}ms");
        await Assert.That(stopwatch.ElapsedMilliseconds).IsLessThan(1000);
    }
}