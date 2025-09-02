using Projects;

namespace BookStore.Tests.Infrastructure;

/// <summary>
/// .NET Aspire 應用程式測試基礎設施
/// </summary>
public class AspireAppFixture : IAsyncLifetime
{
    private DistributedApplication? _app;

    public async Task InitializeAsync()
    {
        // 建立 .NET Aspire Testing 主機
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<BookStore_AppHost>();

        _app = await appHost.BuildAsync();
        await _app.StartAsync();
    }

    /// <summary>
    /// 取得資料庫內容
    /// </summary>
    public async Task<BookStoreDbContext> GetDbContextAsync()
    {
        if (_app == null)
        {
            throw new InvalidOperationException("應用程式尚未初始化");
        }

        // 等待 SQL Server 容器啟動並取得連接字串
        var sqlResource = _app.Services.GetRequiredService<ResourceNotificationService>();
        await sqlResource.WaitForResourceAsync("bookstore-db", KnownResourceStates.Running, CancellationToken.None);

        // 額外等待 SQL Server 完全準備就緒
        await Task.Delay(TimeSpan.FromSeconds(5));

        var connectionString = await _app.GetConnectionStringAsync("bookstore-db");

        var options = new DbContextOptionsBuilder<BookStoreDbContext>()
                      .UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
                      .Options;

        var context = new BookStoreDbContext(options);

        // 確保資料庫存在
        await context.Database.EnsureCreatedAsync();

        return context;
    }

    /// <summary>
    /// 取得資料庫內容（同步版本）
    /// </summary>
    public BookStoreDbContext GetDbContext()
    {
        return GetDbContextAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 取得不使用重試策略的資料庫內容（用於交易測試）
    /// </summary>
    public async Task<BookStoreDbContext> GetDbContextWithoutRetryAsync()
    {
        if (_app == null)
        {
            throw new InvalidOperationException("應用程式尚未初始化");
        }

        var connectionString = await _app.GetConnectionStringAsync("bookstore-db");

        var options = new DbContextOptionsBuilder<BookStoreDbContext>()
                      .UseSqlServer(connectionString) // 不使用 EnableRetryOnFailure
                      .Options;

        var context = new BookStoreDbContext(options);
        return context;
    }

    /// <summary>
    /// 清理資料庫內容
    /// </summary>
    public async Task CleanDatabaseAsync()
    {
        using var context = await GetDbContextAsync();

        // 清理所有書籍資料
        var books = await context.Books.ToListAsync();
        context.Books.RemoveRange(books);
        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }
}