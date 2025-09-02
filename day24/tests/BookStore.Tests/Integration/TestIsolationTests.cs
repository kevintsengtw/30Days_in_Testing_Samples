using BookStore.Tests.Helpers;
using BookStore.Tests.Infrastructure;

namespace BookStore.Tests.Integration;

/// <summary>
/// 測試隔離策略測試
/// </summary>
[Collection("AspireApp")]
public class TestIsolationTests
{
    private readonly AspireAppFixture _fixture;

    public TestIsolationTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task IsolatedTest_使用交易_確保測試隔離()
    {
        // Arrange
        using var dbContext = await _fixture.GetDbContextWithoutRetryAsync();
        using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            // Act & Assert
            var book = await DatabaseTestHelper.CreateTestBookAsync(dbContext);
            book.Id.Should().BeGreaterThan(0);

            // 測試結束後自動回滾，不影響其他測試
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task UniqueDataTest_使用GUID_避免資料衝突()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var book = new Book
        {
            Title = $"測試書籍_{uniqueId}",
            Author = $"測試作者_{uniqueId}",
            Price = 100m
        };

        // Act
        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();

        // Assert
        var savedBook = await dbContext.Books
                                       .FirstOrDefaultAsync(b => b.Title == book.Title);
        savedBook.Should().NotBeNull();
    }
}