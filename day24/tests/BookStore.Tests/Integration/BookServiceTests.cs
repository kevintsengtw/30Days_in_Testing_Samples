using BookStore.Tests.Infrastructure;

namespace BookStore.Tests.Integration;

/// <summary>
/// BookService 業務邏輯測試
/// </summary>
[Collection("AspireApp")]
public class BookServiceTests
{
    private readonly AspireAppFixture _fixture;

    public BookServiceTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    private BookService CreateBookService()
    {
        var dbContext = _fixture.GetDbContext();
        var repository = new EfCoreBookRepository(dbContext);
        return new BookService(repository);
    }

    [Fact]
    public async Task CreateBookAsync_有效資料_應建立書籍並設定建立時間()
    {
        // Arrange
        var bookService = CreateBookService();
        var title = "新書測試";
        var author = "測試作者";
        var price = 350m;

        // Act
        var createdBook = await bookService.CreateBookAsync(title, author, price);

        // Assert
        createdBook.Should().NotBeNull();
        createdBook.Id.Should().BeGreaterThan(0);
        createdBook.Title.Should().Be(title);
        createdBook.Author.Should().Be(author);
        createdBook.Price.Should().Be(price);
        createdBook.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        createdBook.PublishedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Theory]
    [InlineData("", "作者", 100, "書籍標題不可為空")]
    [InlineData("   ", "作者", 100, "書籍標題不可為空")]
    [InlineData("標題", "", 100, "作者不可為空")]
    [InlineData("標題", "   ", 100, "作者不可為空")]
    [InlineData("標題", "作者", 0, "價格必須大於零")]
    [InlineData("標題", "作者", -100, "價格必須大於零")]
    [InlineData("標題", "作者", 15000, "價格不可超過 10,000 元")]
    public async Task CreateBookAsync_無效資料_應拋出ArgumentException(
        string title, string author, decimal price, string expectedMessage)
    {
        // Arrange
        var bookService = CreateBookService();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await bookService.CreateBookAsync(title, author, price));

        exception.Message.Should().Contain(expectedMessage);
    }

    [Fact]
    public async Task UpdateBookPriceAsync_有效價格_應成功更新價格()
    {
        // Arrange
        var bookService = CreateBookService();
        var book = await bookService.CreateBookAsync("測試書籍", "測試作者", 100m);
        var newPrice = 250m;

        // Act
        var updatedBook = await bookService.UpdateBookPriceAsync(book.Id, newPrice);

        // Assert
        updatedBook.Should().NotBeNull();
        updatedBook.Price.Should().Be(newPrice);
        updatedBook.Title.Should().Be("測試書籍", "其他欄位不應變更");
        updatedBook.Author.Should().Be("測試作者", "其他欄位不應變更");
    }

    [Theory]
    [InlineData(0, "書籍 ID 必須大於零")]
    [InlineData(-1, "書籍 ID 必須大於零")]
    public async Task UpdateBookPriceAsync_無效ID_應拋出ArgumentException(int invalidId, string expectedMessage)
    {
        // Arrange
        var bookService = CreateBookService();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await bookService.UpdateBookPriceAsync(invalidId, 100m));

        exception.Message.Should().Contain(expectedMessage);
    }

    [Theory]
    [InlineData(0, "價格必須大於零")]
    [InlineData(-100, "價格必須大於零")]
    [InlineData(15000, "價格不可超過 10,000 元")]
    public async Task UpdateBookPriceAsync_無效價格_應拋出ArgumentException(decimal invalidPrice, string expectedMessage)
    {
        // Arrange
        var bookService = CreateBookService();
        var book = await bookService.CreateBookAsync("測試書籍", "測試作者", 100m);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await bookService.UpdateBookPriceAsync(book.Id, invalidPrice));

        exception.Message.Should().Contain(expectedMessage);
    }

    [Fact]
    public async Task UpdateBookPriceAsync_不存在的書籍_應拋出InvalidOperationException()
    {
        // Arrange
        var bookService = CreateBookService();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await bookService.UpdateBookPriceAsync(99999, 100m));

        exception.Message.Should().Contain("找不到 ID 為 99999 的書籍");
    }

    [Fact]
    public async Task SearchBooksByAuthorAsync_部分匹配_應回傳所有匹配的書籍()
    {
        // Arrange
        await _fixture.CleanDatabaseAsync(); // 清理資料庫以避免資料污染

        var bookService = CreateBookService();
        await bookService.CreateBookAsync("書籍1", "張三", 100m);
        await bookService.CreateBookAsync("書籍2", "張三豐", 200m);
        await bookService.CreateBookAsync("書籍3", "李四", 300m);

        // Act
        var books = await bookService.SearchBooksByAuthorAsync("張");

        // Assert
        books.Should().HaveCount(2);
        books.Should().Contain(b => b.Author == "張三");
        books.Should().Contain(b => b.Author == "張三豐");
        books.Should().NotContain(b => b.Author == "李四");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task SearchBooksByAuthorAsync_空白或Null搜尋條件_應回傳空集合(string searchTerm)
    {
        // Arrange
        var bookService = CreateBookService();

        // Act
        var books = await bookService.SearchBooksByAuthorAsync(searchTerm);

        // Assert
        books.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRecommendedBooksAsync_依業務邏輯_應回傳合理價格範圍的書籍()
    {
        // Arrange
        await _fixture.CleanDatabaseAsync(); // 清理資料庫

        var bookService = CreateBookService();
        await bookService.CreateBookAsync("便宜書", "作者", 50m);   // 低於推薦範圍
        await bookService.CreateBookAsync("推薦書1", "作者", 200m); // 在推薦範圍內
        await bookService.CreateBookAsync("推薦書2", "作者", 500m); // 在推薦範圍內
        await bookService.CreateBookAsync("昂貴書", "作者", 1500m); // 超過推薦範圍

        // Act
        var recommendedBooks = await bookService.GetRecommendedBooksAsync();

        // Assert
        recommendedBooks.Should().HaveCount(2);
        recommendedBooks.Should().OnlyContain(b => b.Price >= 100m && b.Price <= 800m);
        recommendedBooks.Should().Contain(b => b.Title == "推薦書1");
        recommendedBooks.Should().Contain(b => b.Title == "推薦書2");
        recommendedBooks.Should().NotContain(b => b.Title == "便宜書");
        recommendedBooks.Should().NotContain(b => b.Title == "昂貴書");
    }

    [Fact]
    public async Task DeleteBookAsync_存在的書籍_應成功刪除()
    {
        // Arrange
        var bookService = CreateBookService();
        var book = await bookService.CreateBookAsync("要刪除的書", "作者", 100m);

        // Act
        await bookService.DeleteBookAsync(book.Id);

        // Assert
        var deletedBook = await bookService.GetBookAsync(book.Id);
        deletedBook.Should().BeNull("書籍應該已被刪除");
    }

    [Fact]
    public async Task DeleteBookAsync_無效ID_應拋出ArgumentException()
    {
        // Arrange
        var bookService = CreateBookService();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await bookService.DeleteBookAsync(0));

        exception.Message.Should().Contain("書籍 ID 必須大於零");
    }

    [Fact]
    public async Task GetAllBooksAsync_有多本書籍_應回傳所有書籍()
    {
        // Arrange
        var bookService = CreateBookService();
        await bookService.CreateBookAsync("書籍A", "作者A", 100m);
        await bookService.CreateBookAsync("書籍B", "作者B", 200m);

        // Act
        var allBooks = await bookService.GetAllBooksAsync();

        // Assert
        allBooks.Should().HaveCountGreaterThanOrEqualTo(2);
        allBooks.Should().Contain(b => b.Title == "書籍A");
        allBooks.Should().Contain(b => b.Title == "書籍B");
    }
}