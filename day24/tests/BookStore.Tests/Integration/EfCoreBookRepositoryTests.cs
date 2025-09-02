using BookStore.Tests.Infrastructure;

namespace BookStore.Tests.Integration;

/// <summary>
/// EfCoreBookRepository 整合測試
/// </summary>
[Collection("AspireApp")]
public class EfCoreBookRepositoryTests
{
    private readonly AspireAppFixture _fixture;

    public EfCoreBookRepositoryTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_有效書籍_應成功儲存並回傳含ID的書籍()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        var book = new Book
        {
            Title = "測試書籍",
            Author = "測試作者",
            Price = 299.99m,
            PublishedDate = DateTime.UtcNow
        };

        // Act
        var savedBook = await repository.AddAsync(book);

        // Assert
        savedBook.Should().NotBeNull();
        savedBook.Id.Should().BeGreaterThan(0, "應該有自動產生的 ID");
        savedBook.Title.Should().Be("測試書籍");
        savedBook.Author.Should().Be("測試作者");
        savedBook.Price.Should().Be(299.99m);
        savedBook.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        // 驗證資料確實存在於資料庫中
        var retrievedBook = await repository.GetByIdAsync(savedBook.Id);
        retrievedBook.Should().NotBeNull();
        retrievedBook!.Title.Should().Be("測試書籍");
    }

    [Fact]
    public async Task AddAsync_空白標題_應拋出ArgumentException()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        var book = new Book
        {
            Title = "",
            Author = "測試作者",
            Price = 100m
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await repository.AddAsync(book));

        exception.Message.Should().Contain("書籍標題不可為空");
    }

    [Fact]
    public async Task AddAsync_負數價格_應拋出ArgumentException()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        var book = new Book
        {
            Title = "測試書籍",
            Author = "測試作者",
            Price = -100m
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await repository.AddAsync(book));

        exception.Message.Should().Contain("價格不可為負數");
    }

    [Fact]
    public async Task GetByIdAsync_存在的ID_應回傳正確書籍()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        var book = await repository.AddAsync(new Book
        {
            Title = "查詢測試書籍",
            Author = "查詢測試作者",
            Price = 150m
        });

        // Act
        var retrievedBook = await repository.GetByIdAsync(book.Id);

        // Assert
        retrievedBook.Should().NotBeNull();
        retrievedBook!.Id.Should().Be(book.Id);
        retrievedBook.Title.Should().Be("查詢測試書籍");
        retrievedBook.Author.Should().Be("查詢測試作者");
        retrievedBook.Price.Should().Be(150m);
    }

    [Fact]
    public async Task GetByIdAsync_不存在的ID_應回傳Null()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        // Act
        var retrievedBook = await repository.GetByIdAsync(99999);

        // Assert
        retrievedBook.Should().BeNull();
    }

    [Fact]
    public async Task GetBooksByAuthorAsync_指定作者_應回傳該作者所有書籍()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        var author = "特定作者";
        var book1 = await repository.AddAsync(new Book { Title = "書籍1", Author = author, Price = 100m });
        var book2 = await repository.AddAsync(new Book { Title = "書籍2", Author = author, Price = 200m });
        await repository.AddAsync(new Book { Title = "其他書籍", Author = "其他作者", Price = 300m });

        // Act
        var authorBooks = await repository.GetBooksByAuthorAsync(author);

        // Assert
        authorBooks.Should().HaveCount(2);
        authorBooks.Should().Contain(b => b.Title == "書籍1");
        authorBooks.Should().Contain(b => b.Title == "書籍2");
        authorBooks.Should().NotContain(b => b.Author == "其他作者");
    }

    [Fact]
    public async Task GetExpensiveBooksAsync_指定最低價格_應回傳高價書籍並按價格降冪排序()
    {
        // Arrange
        await _fixture.CleanDatabaseAsync(); // 清理資料庫

        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        await repository.AddAsync(new Book { Title = "便宜書", Author = "作者", Price = 50m });
        await repository.AddAsync(new Book { Title = "中價書", Author = "作者", Price = 300m });
        await repository.AddAsync(new Book { Title = "高價書", Author = "作者", Price = 500m });

        // Act
        var expensiveBooks = await repository.GetExpensiveBooksAsync(200m);

        // Assert
        expensiveBooks.Should().HaveCount(2);
        expensiveBooks.Should().BeInDescendingOrder(b => b.Price);
        expensiveBooks.First().Title.Should().Be("高價書");
        expensiveBooks.Last().Title.Should().Be("中價書");
    }

    [Fact]
    public async Task UpdateAsync_修改書籍資料_應正確更新並設定UpdatedDate()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        var book = await repository.AddAsync(new Book
        {
            Title = "原始標題",
            Author = "原始作者",
            Price = 100m
        });

        // Act
        book.Title = "更新標題";
        book.Price = 200m;
        await repository.UpdateAsync(book);

        // Assert
        var updatedBook = await repository.GetByIdAsync(book.Id);
        updatedBook.Should().NotBeNull();
        updatedBook!.Title.Should().Be("更新標題");
        updatedBook.Price.Should().Be(200m);
        updatedBook.Author.Should().Be("原始作者", "未修改的欄位應保持不變");
        updatedBook.UpdatedDate.Should().NotBeNull();
        updatedBook.UpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task DeleteAsync_存在的書籍_應成功刪除()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        var book = await repository.AddAsync(new Book
        {
            Title = "要刪除的書籍",
            Author = "作者",
            Price = 100m
        });

        // Act
        await repository.DeleteAsync(book.Id);

        // Assert
        var deletedBook = await repository.GetByIdAsync(book.Id);
        deletedBook.Should().BeNull("書籍應該已被刪除");
    }

    [Fact]
    public async Task DeleteAsync_不存在的ID_應拋出InvalidOperationException()
    {
        // Arrange
        using var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await repository.DeleteAsync(99999));

        exception.Message.Should().Contain("找不到 ID 為 99999 的書籍");
    }
}