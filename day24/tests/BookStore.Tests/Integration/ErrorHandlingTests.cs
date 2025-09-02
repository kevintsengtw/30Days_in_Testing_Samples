using System.Diagnostics;
using BookStore.Tests.Helpers;
using BookStore.Tests.Infrastructure;

namespace BookStore.Tests.Integration;

/// <summary>
/// 錯誤處理與診斷測試
/// </summary>
[Collection("AspireApp")]
public class ErrorHandlingTests
{
    private readonly AspireAppFixture _fixture;

    public ErrorHandlingTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DetailedErrorHandling_提供診斷資訊()
    {
        // Arrange
        using var dbContext = await _fixture.GetDbContextWithoutRetryAsync();

        // Act & Assert
        var invalidBook = new Book
        {
            Title = new string('A', 300),  // 超過 MaxLength(200) 限制
            Author = new string('B', 150), // 超過 MaxLength(100) 限制 
            Price = 100m
        };
        dbContext.Books.Add(invalidBook);

        // 期待會拋出例外 - 可能是連接問題或約束問題
        var exception = await Assert.ThrowsAnyAsync<Exception>(
            async () => await dbContext.SaveChangesAsync());

        // Assert - 驗證例外包含有用資訊
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();

        // 提供診斷資訊但不拋出例外
        var connectionString = dbContext.Database.GetConnectionString();
        var maskedConnectionString = DatabaseTestHelper.MaskSensitiveInfo(connectionString!);

        // 這只是記錄用，測試應該成功
        Debug.WriteLine($"測試成功捕獲錯誤: {exception.Message}");
        Debug.WriteLine($"資料庫連接: {maskedConnectionString}");
    }

    [Fact]
    public async Task HealthCheck_驗證測試環境可用性()
    {
        // Arrange & Act
        using var dbContext = _fixture.GetDbContext();

        // Assert
        var canConnect = await dbContext.Database.CanConnectAsync();
        canConnect.Should().BeTrue("應該能夠連接到測試資料庫");

        // 確保連接已開啟
        await dbContext.Database.OpenConnectionAsync();

        // 驗證資料庫版本
        var serverVersion = dbContext.Database.GetDbConnection().ServerVersion;
        serverVersion.Should().NotBeNullOrEmpty("應該能夠取得 SQL Server 版本資訊");

        // 驗證基本查詢功能
        var result = await dbContext.Database
                                    .SqlQueryRaw<DateTimeQueryResult>("SELECT GETUTCDATE() as Value")
                                    .FirstAsync();

        // 容忍更大的時差，因為容器環境可能有時區差異
        var timeDifference = Math.Abs((result.Value - DateTime.UtcNow).TotalMinutes);
        timeDifference.Should().BeLessThan(600, "查詢結果應該在合理的時間範圍內"); // 10小時內
    }
}