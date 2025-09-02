namespace BookStore.Tests.Helpers;

/// <summary>
/// 資料庫測試輔助類別
/// </summary>
public static class DatabaseTestHelper
{
    /// <summary>
    /// 建立測試書籍
    /// </summary>
    public static async Task<Book> CreateTestBookAsync(BookStoreDbContext context,
                                                       string title = "測試書籍",
                                                       string author = "測試作者",
                                                       decimal price = 100m)
    {
        var book = new Book { Title = title, Author = author, Price = price };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        return book;
    }

    /// <summary>
    /// 清理測試資料
    /// </summary>
    public static async Task CleanupTestDataAsync(BookStoreDbContext context)
    {
        // 只清理測試產生的資料
        var testBooks = context.Books.Where(b =>
                                                b.Title.StartsWith("測試") || b.Author.StartsWith("測試"));
        context.Books.RemoveRange(testBooks);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 遮蔽敏感資訊
    /// </summary>
    public static string MaskSensitiveInfo(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return string.Empty;
        }

        // 遮蔽敏感資訊，只保留診斷需要的部分
        return connectionString.Split(';')
                               .Where(part => !part.Contains("Password", StringComparison.OrdinalIgnoreCase))
                               .Where(part => !part.Contains("User Id", StringComparison.OrdinalIgnoreCase))
                               .Aggregate((a, b) => $"{a};{b}");
    }
}