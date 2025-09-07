namespace TUnit.Advanced.Integration.Tests;

/// <summary>
/// 測試容器設置類別
/// 展示使用 [Before(Assembly)] 和 [After(Assembly)] 管理 Testcontainers 的最佳實踐
/// </summary>
public static class TestContainersSetup
{
    public static PostgreSqlContainer? PostgreSqlContainer { get; private set; }
    public static RedisContainer? RedisContainer { get; private set; }
    public static KafkaContainer? KafkaContainer { get; private set; }
    public static INetwork? Network { get; private set; }

    /// <summary>
    /// Assembly 層級的設置
    /// 在整個測試組件開始執行前設置所有容器
    /// 這樣所有測試類別都可以共用相同的容器實例
    /// </summary>
    [Before(Assembly)]
    public static async Task SetupTestContainers()
    {
        Console.WriteLine("🚀 開始設置測試容器基礎設施...");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 1. 建立測試網路
            Network = new NetworkBuilder()
                      .WithName("tunit-test-network")
                      .Build();

            await Network.CreateAsync();
            Console.WriteLine($"✓ 測試網路已建立: {Network.Name}");

            // 2. 建立 PostgreSQL 容器
            PostgreSqlContainer = new PostgreSqlBuilder()
                                  .WithDatabase("test_db")
                                  .WithUsername("test_user")
                                  .WithPassword("test_password")
                                  .WithNetwork(Network)
                                  .WithNetworkAliases("postgres-db")
                                  .WithCleanUp(true)
                                  .Build();

            await PostgreSqlContainer.StartAsync();
            Console.WriteLine($"✓ PostgreSQL 容器已啟動 ({PostgreSqlContainer.GetMappedPublicPort(5432)} -> 5432)");

            // 3. 建立 Redis 容器
            RedisContainer = new RedisBuilder()
                             .WithNetwork(Network)
                             .WithNetworkAliases("redis-cache")
                             .WithCleanUp(true)
                             .Build();

            await RedisContainer.StartAsync();
            Console.WriteLine($"✓ Redis 容器已啟動 ({RedisContainer.GetMappedPublicPort(6379)} -> 6379)");

            // 4. 建立 Kafka 容器
            KafkaContainer = new KafkaBuilder()
                             .WithNetwork(Network)
                             .WithNetworkAliases("kafka-broker")
                             .WithCleanUp(true)
                             .Build();

            await KafkaContainer.StartAsync();
            Console.WriteLine($"✓ Kafka 容器已啟動: {KafkaContainer.GetBootstrapAddress()}");

            stopwatch.Stop();
            Console.WriteLine($"🎉 測試容器基礎設施設置完成！耗時: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"❌ 測試容器設置失敗: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Assembly 層級的清理
    /// 在整個測試組件執行完畢後清理所有容器
    /// </summary>
    [After(Assembly)]
    public static async Task TeardownTestContainers()
    {
        Console.WriteLine();
        Console.WriteLine("🧹 開始清理測試容器基礎設施...");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 按照相依性順序進行清理：應用容器 -> 中介軟體 -> 資料庫 -> 網路

            if (KafkaContainer != null)
            {
                await KafkaContainer.DisposeAsync();
                Console.WriteLine("✓ Kafka 容器已停止並清理");
            }

            if (RedisContainer != null)
            {
                await RedisContainer.DisposeAsync();
                Console.WriteLine("✓ Redis 容器已停止並清理");
            }

            if (PostgreSqlContainer != null)
            {
                await PostgreSqlContainer.DisposeAsync();
                Console.WriteLine("✓ PostgreSQL 容器已停止並清理");
            }

            if (Network != null)
            {
                await Network.DeleteAsync();
                Console.WriteLine("✓ 測試網路已刪除");
            }

            stopwatch.Stop();
            Console.WriteLine($"🎉 測試容器基礎設施清理完成！耗時: {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"❌ 測試容器清理過程中發生錯誤: {ex.Message}");
            // 不重新拋出例外，避免影響測試結果
        }
    }

    /// <summary>
    /// 取得測試設定字典
    /// 提供給需要設定的測試使用
    /// </summary>
    public static Dictionary<string, string> GetTestConfiguration()
    {
        if (PostgreSqlContainer == null || RedisContainer == null)
        {
            throw new InvalidOperationException("測試容器尚未初始化，請確保 SetupTestContainers 已執行");
        }

        return new Dictionary<string, string>
        {
            ["ConnectionStrings:DefaultConnection"] = PostgreSqlContainer.GetConnectionString(),
            ["ConnectionStrings:Redis"] = RedisContainer.GetConnectionString(),
            ["ConnectionStrings:Kafka"] = KafkaContainer?.GetBootstrapAddress() ?? "",
            ["Environment"] = "Testing",
            ["TestNetwork"] = Network?.Name ?? "tunit-test-network"
        };
    }

    /// <summary>
    /// 驗證所有容器是否正常運行
    /// </summary>
    public static bool AreAllContainersHealthy()
    {
        return PostgreSqlContainer?.State == TestcontainersStates.Running &&
               RedisContainer?.State == TestcontainersStates.Running &&
               KafkaContainer?.State == TestcontainersStates.Running;
    }
}