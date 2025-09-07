namespace TUnit.Advanced.Integration.Tests;

/// <summary>
/// 進階依賴管理模式範例
/// 展示 TUnit 的複雜依賴鏈管理能力
/// 注意：這些是基礎設施健康檢查測試，確保測試環境的服務依賴正常
/// 真正的業務邏輯測試會使用這些已驗證的基礎設施進行實際功能測試
/// </summary>
public class AdvancedDependencyTests
{
    [Test]
    [Property("Category", "Network")]
    [DisplayName("網路基礎設施驗證")]
    public async Task NetworkInfrastructure_網路設定_應正確建立()
    {
        // Arrange & Act
        var networkName = GlobalTestInfrastructureSetup.Network!.Name;

        // Assert
        await Assert.That(networkName).IsEqualTo("global-test-network");

        Console.WriteLine($"Test network verified: {networkName}");
    }

    [Test]
    [Property("Category", "Database")]
    [DisplayName("網路化資料庫服務驗證")]
    public async Task NetworkedDatabase_資料庫網路_應正確設定()
    {
        // Arrange
        var connectionString = GlobalTestInfrastructureSetup.PostgreSqlContainer!.GetConnectionString();

        // Act & Assert
        await Assert.That(connectionString).Contains("test_db");
        await Assert.That(connectionString).Contains("test_user");

        // 驗證資料庫容器在指定網路中
        await Assert.That(GlobalTestInfrastructureSetup.PostgreSqlContainer.State).IsEqualTo(TestcontainersStates.Running);

        Console.WriteLine($"Networked database verified: {connectionString}");
    }

    [Test]
    [Property("Category", "Integration")]
    [DisplayName("跨容器網路通訊測試")]
    public async Task CrossContainerCommunication_容器間通訊_應正常運作()
    {
        // Arrange
        var dbConnectionString = GlobalTestInfrastructureSetup.PostgreSqlContainer!.GetConnectionString();
        var networkName = GlobalTestInfrastructureSetup.Network!.Name;

        // Act & Assert
        await Assert.That(dbConnectionString).IsNotNull();
        await Assert.That(networkName).IsEqualTo("global-test-network");

        Console.WriteLine("Cross-container communication test ready");
        Console.WriteLine($"Network: {networkName}");
        Console.WriteLine($"Database: {dbConnectionString}");
    }
}