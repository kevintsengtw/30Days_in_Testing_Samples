using BookStore.Tests.Helpers;
using BookStore.Tests.Infrastructure;
using BookStore.Tests.Models;

namespace BookStore.Tests.Integration;

/// <summary>
/// 資料庫特定功能測試
/// </summary>
[Collection("AspireApp")]
public class DatabaseFeatureTests
{
    private readonly AspireAppFixture _fixture;

    public DatabaseFeatureTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DatabaseMigration_執行遷移腳本_應正確建立資料表結構()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();

        // Act
        await dbContext.Database.MigrateAsync();

        // Assert
        var canConnect = await dbContext.Database.CanConnectAsync();
        canConnect.Should().BeTrue("應該能夠連接到資料庫");

        // 驗證特定資料表和欄位是否存在
        var tableResult = await dbContext.Database
                                         .SqlQueryRaw<QueryResult>(@"
                SELECT COUNT(*) as Value
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME = 'Books' AND TABLE_SCHEMA = 'dbo'")
                                         .FirstAsync();

        tableResult.Value.Should().Be(1, "Books 資料表應該存在");

        // 驗證欄位結構
        var columnResult = await dbContext.Database
                                          .SqlQueryRaw<QueryResult>(@"
                SELECT COUNT(*) as Value
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'Books' AND TABLE_SCHEMA = 'dbo'")
                                          .FirstAsync();

        columnResult.Value.Should().BeGreaterThan(0, "Books 資料表應該有欄位定義");
    }

    [Fact]
    public async Task SqlQuery_執行原生SQL_應正確處理複雜查詢()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        await TestDataSeeder.SeedBasicDataAsync(dbContext);

        // Act - 執行複雜的 SQL 查詢
        var results = await dbContext.Database
                                     .SqlQueryRaw<BookSummary>(@"
                SELECT 
                    Author,
                    COUNT(*) as BookCount,
                    AVG(CAST(Price as FLOAT)) as AveragePrice,
                    MAX(Price) as MaxPrice
                FROM Books 
                GROUP BY Author
                HAVING COUNT(*) > 0
                ORDER BY AveragePrice DESC")
                                     .ToListAsync();

        // Assert
        results.Should().NotBeEmpty("應該有統計結果");

        foreach (var result in results)
        {
            result.Author.Should().NotBeNullOrWhiteSpace("作者名稱不應為空");
            result.BookCount.Should().BeGreaterThan(0);
            result.AveragePrice.Should().BeGreaterThan(0);
            result.MaxPrice.Should().BeGreaterThan(0);
        }
    }
}