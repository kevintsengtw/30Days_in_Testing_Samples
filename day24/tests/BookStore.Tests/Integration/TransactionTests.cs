using BookStore.Tests.Infrastructure;

namespace BookStore.Tests.Integration;

/// <summary>
/// 交易處理與並發測試
/// </summary>
[Collection("AspireApp")]
public class TransactionTests
{
    private readonly AspireAppFixture _fixture;

    public TransactionTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateBooks_使用交易_失敗時應完整回滾()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        using var transaction = await dbContext.Database.BeginTransactionAsync();

        var validBook = new Book { Title = "有效書籍", Author = "作者", Price = 100m };
        var invalidBook = new Book { Title = "無效書籍", Author = "", Price = -100m }; // 假設 Author 不能空白

        try
        {
            // Act
            dbContext.Books.Add(validBook);
            await dbContext.SaveChangesAsync();

            dbContext.Books.Add(invalidBook);
            await dbContext.SaveChangesAsync(); // 這裡可能會因為驗證規則失敗

            await transaction.CommitAsync();
            Assert.Fail("預期會拋出例外"); // 如果沒有例外就是測試失敗
        }
        catch (Exception)
        {
            // Assert
            await transaction.RollbackAsync();

            // 驗證交易回滾後，所有變更都被撤銷
            var bookCount = await dbContext.Books.CountAsync(b => b.Title == "有效書籍");
            bookCount.Should().Be(0, "交易回滾後，所有資料都應該被撤銷");
        }
    }

    [Fact]
    public async Task ConcurrentBookCreation_多執行緒存取_應保持資料一致性()
    {
        // Arrange
        await _fixture.CleanDatabaseAsync(); // 清理資料庫以避免資料表已存在錯誤

        var tasks = new List<Task<int>>();
        var bookTitles = new List<string>();

        // Act - 模擬 10 個並發的書籍建立操作
        for (var i = 0; i < 10; i++)
        {
            var title = $"並發測試書籍 {i:D2}";
            bookTitles.Add(title);

            tasks.Add(Task.Run(async () =>
            {
                using var dbContext = _fixture.GetDbContext();
                var book = new Book
                {
                    Title = title,
                    Author = "並發作者",
                    Price = 99.99m
                };

                dbContext.Books.Add(book);
                await dbContext.SaveChangesAsync();
                return book.Id;
            }));
        }

        var bookIds = await Task.WhenAll(tasks);

        // Assert
        bookIds.Should().HaveCount(10, "所有並發操作都應該成功");
        bookIds.Should().OnlyHaveUniqueItems("每本書都應該有唯一的 ID");

        // 驗證資料庫中的資料完整性
        using var verifyContext = _fixture.GetDbContext();
        var savedBooks = await verifyContext.Books
                                            .Where(b => b.Author == "並發作者")
                                            .ToListAsync();

        savedBooks.Should().HaveCount(10, "所有書籍都應該被正確儲存");

        foreach (var expectedTitle in bookTitles)
        {
            savedBooks.Should().Contain(b => b.Title == expectedTitle,
                                        $"應該包含書籍: {expectedTitle}");
        }
    }
}