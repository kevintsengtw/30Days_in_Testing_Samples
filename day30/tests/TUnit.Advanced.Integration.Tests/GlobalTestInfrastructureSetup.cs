namespace TUnit.Advanced.Integration.Tests;

/// <summary>
/// 全域測試基礎設施設置
/// 專門處理 Assembly Level 的容器管理
/// </summary>
public static class GlobalTestInfrastructureSetup
{
    public static PostgreSqlContainer? PostgreSqlContainer { get; private set; }
    public static RedisContainer? RedisContainer { get; private set; }
    public static KafkaContainer? KafkaContainer { get; private set; }
    public static INetwork? Network { get; private set; }

    [Before(Assembly)]
    public static async Task SetupGlobalInfrastructure()
    {
        Console.WriteLine("=== 開始設置全域測試基礎設施 ===");

        // 建立網路
        Network = new NetworkBuilder()
                  .WithName("global-test-network")
                  .Build();

        await Network.CreateAsync();
        Console.WriteLine($"✓ 測試網路已建立: {Network.Name}");

        // 建立 PostgreSQL 容器
        PostgreSqlContainer = new PostgreSqlBuilder()
                              .WithDatabase("test_db")
                              .WithUsername("test_user")
                              .WithPassword("test_password")
                              .WithNetwork(Network)
                              .WithCleanUp(true)
                              .Build();

        await PostgreSqlContainer.StartAsync();
        Console.WriteLine($"✓ PostgreSQL 容器已啟動: {PostgreSqlContainer.GetConnectionString()}");

        // 建立 Redis 容器
        RedisContainer = new RedisBuilder()
                         .WithNetwork(Network)
                         .WithCleanUp(true)
                         .Build();

        await RedisContainer.StartAsync();
        Console.WriteLine($"✓ Redis 容器已啟動: {RedisContainer.GetConnectionString()}");

        // 建立 Kafka 容器
        KafkaContainer = new KafkaBuilder()
                         .WithNetwork(Network)
                         .WithCleanUp(true)
                         .Build();

        await KafkaContainer.StartAsync();
        Console.WriteLine($"✓ Kafka 容器已啟動: {KafkaContainer.GetBootstrapAddress()}");

        Console.WriteLine("=== 全域測試基礎設施設置完成 ===");
    }

    [After(Assembly)]
    public static async Task TeardownGlobalInfrastructure()
    {
        Console.WriteLine("=== 開始清理全域測試基礎設施 ===");

        if (KafkaContainer != null)
        {
            await KafkaContainer.DisposeAsync();
            Console.WriteLine("✓ Kafka 容器已停止");
        }

        if (RedisContainer != null)
        {
            await RedisContainer.DisposeAsync();
            Console.WriteLine("✓ Redis 容器已停止");
        }

        if (PostgreSqlContainer != null)
        {
            await PostgreSqlContainer.DisposeAsync();
            Console.WriteLine("✓ PostgreSQL 容器已停止");
        }

        if (Network != null)
        {
            await Network.DeleteAsync();
            Console.WriteLine("✓ 測試網路已刪除");
        }

        Console.WriteLine("=== 全域測試基礎設施清理完成 ===");
    }
}