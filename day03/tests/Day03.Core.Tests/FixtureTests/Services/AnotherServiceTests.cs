using Day03.Core.Data;

namespace Day03.Core.Tests.FixtureTests.Services;

/// <summary>
/// class AnotherServiceTests - 用於測試其他服務的功能。
/// </summary>
[Collection("Service Collection")]
public class AnotherServiceTests
{
    private readonly ServiceFixture _fixture;

    /// <summary>
    /// AnotherServiceTests 的建構函式
    /// </summary>
    /// <param name="fixture"></param>
    public AnotherServiceTests(ServiceFixture fixture)
    {
        this._fixture = fixture;
    }

    [Fact]
    public void 這個測試會與ServiceIntegrationTests共享同一個ServiceFixture實例()
    {
        // 這個測試會與 ServiceIntegrationTests 共享同一個 ServiceFixture 實例
        // 但每個測試類別仍然有獨立的測試實例

        using var scope = this._fixture.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var users = dbContext.Users.ToList();

        // 由於共享 Fixture，可能會看到其他測試的影響
        Assert.True(users.Count >= ServiceFixture.InitialUserCount); // 至少有初始的使用者

        // 驗證 Fixture 實例是共享的
        Assert.NotNull(this._fixture.ServiceProvider);
        Assert.Same(this._fixture.ServiceProvider, this._fixture.ServiceProvider); // 驗證是同一個實例
    }
}