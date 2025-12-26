using Projects;

namespace BookStore.Tests.Infrastructure;

/// <summary>
/// .NET Aspire 應用程式測試基礎設施
/// </summary>
public class AspireAppFixture : IAsyncLifetime
{
    private DistributedApplication? _app;
    private bool _databaseInitialized = false;
    private readonly object _initLock = new object();

    public async Task InitializeAsync()
    {
        // 建立 .NET Aspire Testing 主機
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<BookStore_AppHost>();

        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        // 初始化資料庫（只執行一次）
        await InitializeDatabaseAsync();
    }

    /// <summary>
    /// 初始化資料庫結構（只執行一次）
    /// </summary>
    private async Task InitializeDatabaseAsync()
    {
        lock (_initLock)
        {
            if (_databaseInitialized)
                return;

            _databaseInitialized = true;
        }

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

        using var context = new BookStoreDbContext(options);

        // 確保資料庫存在（只在初始化時執行一次）
        await context.Database.EnsureCreatedAsync();
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

        var connectionString = await _app.GetConnectionStringAsync("bookstore-db");

        var options = new DbContextOptionsBuilder<BookStoreDbContext>()
                      .UseSqlServer(connectionString, options => options.EnableRetryOnFailure())
                      .Options;

        var context = new BookStoreDbContext(options);
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
    /// 清理資料庫內容（移除所有書籍）
    /// </summary>
    public async Task CleanDatabaseAsync()
    {
        using var context = await GetDbContextAsync();

        // 使用 SQL 直接刪除以提高效率
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Books");
        
        // 重置 IDENTITY
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Books', RESEED, 0)");
    }

    public async Task DisposeAsync()
    {
        if (_app != null)
        {
            await _app.DisposeAsync();
        }
    }
}