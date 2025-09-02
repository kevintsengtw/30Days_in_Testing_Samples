using BookStore.Tests.Infrastructure;

namespace BookStore.Tests.Integration;

/// <summary>
/// 簡化版書店資料庫測試 - 展示 .NET Aspire Testing 概念
/// 注意：實際執行需要 Docker 和 SQL Server 容器
/// </summary>
[Collection("AspireApp")]
public class SimplifiedBookStoreTests
{
    private readonly AspireAppFixture _fixture;

    public SimplifiedBookStoreTests(AspireAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void CreateBook_展示基本模型結構_應正確建立物件()
    {
        // Arrange
        var newBook = new Book
        {
            Title = "測試書籍",
            Author = "測試作者",
            Price = 299.99m,
            PublishedDate = DateTime.UtcNow
        };

        // Act & Assert
        newBook.Title.Should().Be("測試書籍");
        newBook.Author.Should().Be("測試作者");
        newBook.Price.Should().Be(299.99m);
        newBook.PublishedDate.Should().NotBeNull();
    }

    [Fact]
    public void DbContext_展示配置結構_應正確建立Context()
    {
        // Arrange & Act
        var dbContext = _fixture.GetDbContext();

        // Assert
        dbContext.Should().NotBeNull();
        dbContext.Books.Should().NotBeNull();
    }

    [Fact]
    public void TestDataSeeder_展示資料播種邏輯_應建立測試資料()
    {
        // Arrange
        var books = new[]
        {
            new Book { Title = "C# 程式設計", Author = "張三", Price = 450.00m },
            new Book { Title = ".NET Core 實戰", Author = "李四", Price = 520.00m },
            new Book { Title = "ASP.NET Core 開發", Author = "王五", Price = 480.00m }
        };

        // Act
        var expensiveBooks = books
                             .Where(b => b.Price > 400)
                             .OrderByDescending(b => b.Price)
                             .ToList();

        // Assert
        expensiveBooks.Should().NotBeEmpty("應該要有價格超過 400 的書籍");
        expensiveBooks.Should().HaveCount(3, "所有測試書籍價格都超過 400");
        expensiveBooks.Should().BeInDescendingOrder(b => b.Price, "結果應該按價格降冪排序");
    }
}