namespace BookStore.Tests.Infrastructure;

/// <summary>
/// 定義測試集合，讓所有整合測試共享同一個 AspireAppFixture
/// </summary>
[CollectionDefinition("AspireApp")]
public class AspireAppCollectionDefinition : ICollectionFixture<AspireAppFixture>
{
    // 這個類別不需要任何實作
    // 它只是用來定義測試集合和關聯的 Fixture
}