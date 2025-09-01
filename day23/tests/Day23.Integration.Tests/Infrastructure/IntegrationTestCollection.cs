namespace Day23.Integration.Tests.Infrastructure;

/// <summary>
/// 整合測試集合定義 - 所有整合測試共享同一個容器
/// </summary>
[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<TestWebApplicationFactory>
{
    /// <summary>
    /// 集合名稱常數
    /// </summary>
    public const string Name = "Integration Tests";

    // 這個類別不需要任何實作
    // 它只是用來定義 Collection Fixture
}