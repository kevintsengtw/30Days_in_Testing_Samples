namespace TUnit.Advanced.Lifecycle.Tests;

/// <summary>
/// 展示 Properties 屬性標記與測試過濾功能
/// </summary>
public class PropertiesTests
{
    [Test]
    [Property("Category", "Database")]
    [Property("Priority", "High")]
    public async Task DatabaseTest_高優先級_應能透過屬性過濾()
    {
        // 這個測試可以透過 Category=Database 或 Priority=High 來過濾執行
        await Assert.That(true).IsTrue();
    }

    [Test]
    [Property("Category", "Unit")]
    [Property("Priority", "Medium")]
    public async Task UnitTest_中等優先級_基本驗證()
    {
        await Assert.That(1 + 1).IsEqualTo(2);
    }

    [Test]
    [Property("Category", "Integration")]
    [Property("Priority", "Low")]
    [Property("Environment", "Development")]
    public async Task IntegrationTest_低優先級_僅開發環境執行()
    {
        // 可以透過多個屬性組合來精確過濾測試
        await Assert.That("Hello World").Contains("World");
    }
}