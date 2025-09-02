namespace BookStore.Tests.Helpers;

/// <summary>
/// 測試資料播種器
/// </summary>
public static class TestDataSeeder
{
    /// <summary>
    /// 播種基礎測試資料
    /// </summary>
    public static async Task SeedBasicDataAsync(BookStoreDbContext context)
    {
        // 檢查是否已有測試資料
        var existingCount = await context.Books.CountAsync(b =>
                                                               b.Author == "張三" || b.Author == "李四" || b.Author == "王五");

        if (existingCount > 0)
        {
            return; // 資料已存在，跳過初始化
        }

        var books = new[]
        {
            new Book { Title = "C# 程式設計", Author = "張三", Price = 450.00m },
            new Book { Title = ".NET Core 實戰", Author = "李四", Price = 520.00m },
            new Book { Title = "ASP.NET Core 開發", Author = "王五", Price = 480.00m }
        };

        context.Books.AddRange(books);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 清理測試資料
    /// </summary>
    public static async Task CleanupDataAsync(BookStoreDbContext context)
    {
        // 清理所有測試相關資料
        var testBooks = await context.Books
                                     .Where(b => b.Author.StartsWith("測試") ||
                                                 b.Author == "張三" ||
                                                 b.Author == "李四" ||
                                                 b.Author == "王五")
                                     .ToListAsync();

        context.Books.RemoveRange(testBooks);
        await context.SaveChangesAsync();
    }
}