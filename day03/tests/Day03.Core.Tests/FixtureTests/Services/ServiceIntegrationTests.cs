using Day03.Core.Data;

namespace Day03.Core.Tests.FixtureTests.Services;

/// <summary>
/// class ServiceIntegrationTests - 用於測試服務的整合功能。
/// </summary>
[Collection("Service Collection")]
public class ServiceIntegrationTests
{
    private readonly ServiceFixture _fixture;

    /// <summary>
    /// ServiceIntegrationTests 的建構函式
    /// </summary>
    public ServiceIntegrationTests(ServiceFixture fixture)
    {
        this._fixture = fixture;
    }

    [Fact]
    public void DbContext_應該包含初始測試資料()
    {
        // Arrange
        using var scope = this._fixture.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Act
        var users = dbContext.Users.ToList();

        // Assert
        Assert.Equal(ServiceFixture.InitialUserCount, users.Count);
        Assert.Contains(users, u => u.Name == "Service User 1");
        Assert.Contains(users, u => u.Name == "Service User 2");
    }

    [Fact]
    public void AddUser_應該成功新增使用者()
    {
        // Arrange
        using var scope = this._fixture.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 先確認初始狀態
        var initialCount = dbContext.Users.Count();

        var newUser = new User
        {
            Name = "Integration Test User",
            Email = "integration@example.com",
            Age = 30
        };

        // Act
        dbContext.Users.Add(newUser);
        dbContext.SaveChanges();

        // Assert
        var finalCount = dbContext.Users.Count();
        Assert.Equal(initialCount + 1, finalCount);
        Assert.Contains(dbContext.Users, u => u.Name == "Integration Test User");
    }
}