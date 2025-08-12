using Day03.Core.Data;

namespace Day03.Core.Tests.FixtureTests.Services;

/// <summary>
/// class ServiceFixture - 用於測試服務的功能。
/// </summary>
public class ServiceFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }
    public static int InitialUserCount => 2;

    /// <summary>
    /// ServiceFixture 的建構函式
    /// </summary>
    public ServiceFixture()
    {
        var databaseName = $"TestServiceDb_{Guid.NewGuid()}";
        var services = new ServiceCollection();

        // 註冊測試所需的服務
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(databaseName));

        // 建立服務提供者
        this.ServiceProvider = services.BuildServiceProvider();

        // 初始化資料庫
        this.InitializeDatabase();
    }

    /// <summary>
    /// 初始化資料庫
    /// </summary>
    private void InitializeDatabase()
    {
        using var scope = this.ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        // 清除現有資料並重新植入
        dbContext.Users.RemoveRange(dbContext.Users);
        dbContext.SaveChanges();

        this.SeedData(dbContext);
    }

    /// <summary>
    /// 初始化測試資料
    /// </summary>
    private void SeedData(AppDbContext dbContext)
    {
        var testUsers = new[]
        {
            new User { Name = "Service User 1", Email = "service1@example.com", Age = 25 },
            new User { Name = "Service User 2", Email = "service2@example.com", Age = 30 }
        };

        dbContext.Users.AddRange(testUsers);
        dbContext.SaveChanges();
    }

    /// <summary>
    /// 重置資料庫
    /// </summary>
    public void ResetDatabase()
    {
        this.InitializeDatabase();
    }

    /// <summary>
    /// 獲取服務
    /// </summary>
    public T GetService<T>() where T : notnull
    {
        return this.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 釋放資源
    /// </summary>
    public void Dispose()
    {
        if (this.ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}