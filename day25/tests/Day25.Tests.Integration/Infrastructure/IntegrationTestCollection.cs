namespace Day25.Tests.Integration.Infrastructure;

/// <summary>
/// 整合測試集合定義
/// 使用 Collection Fixture 在所有測試類別間共享 AspireAppFixture
/// </summary>
[CollectionDefinition(Name)]
public class IntegrationTestCollection : ICollectionFixture<AspireAppFixture>
{
    /// <summary>
    /// 測試集合名稱
    /// </summary>
    public const string Name = "Integration Tests";

    // 這個類別不需要實作任何程式碼
    // 它只是用來定義 Collection Fixture
}