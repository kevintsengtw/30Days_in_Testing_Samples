using System;
using Day03.Core.Data;

namespace Day03.Core.Tests.FixtureTests.Repositories;

/// <summary>
/// class DatabaseFixture - 用於測試資料庫的功能。
/// </summary>
public class DatabaseFixture
{
    public AppDbContext DbContext { get; }

    /// <summary>
    /// DatabaseFixture 的建構函式
    /// </summary>
    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
                      .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                      .Options;

        this.DbContext = new AppDbContext(options);
        this.DbContext.Database.EnsureCreated();

        // 初始化測試資料
        this.SeedTestData();
    }

    /// <summary>
    /// 初始化測試資料
    /// </summary>
    private void SeedTestData()
    {
        var testUsers = new[]
        {
            new User { Name = "Test User 1", Email = "test1@example.com", Age = 25 },
            new User { Name = "Test User 2", Email = "test2@example.com", Age = 30 },
            new User { Name = "Admin User", Email = "admin@company.com", Age = 35 }
        };

        this.DbContext.Users.AddRange(testUsers);
        this.DbContext.SaveChanges();
    }

    /// <summary>
    /// 清理測試資料
    /// </summary>
    public void CleanupData()
    {
        // 在需要時清理測試資料
        this.DbContext.Users.RemoveRange(this.DbContext.Users);
        this.DbContext.SaveChanges();
    }

    /// <summary>
    /// 釋放資源
    /// </summary>
    public void Dispose()
    {
        this.DbContext.Dispose();
    }
}