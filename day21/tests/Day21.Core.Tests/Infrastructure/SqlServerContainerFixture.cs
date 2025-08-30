namespace Day21.Core.Tests.Infrastructure;

/// <summary>
/// SQL Server 容器 Fixture，負責管理測試用的 SQL Server 容器生命週期
/// </summary>
public class SqlServerContainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;

    public SqlServerContainerFixture()
    {
        _container = new MsSqlBuilder()
                     .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                     .WithPassword("Test123456!")
                     .WithCleanUp(true)
                     .Build();
    }

    /// <summary>
    /// 取得連線字串
    /// </summary>
    public static string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// 初始化容器
    /// </summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();

        // 等待容器完全啟動
        await Task.Delay(2000);

        Console.WriteLine($"SQL Server 容器已啟動，連線字串：{ConnectionString}");
    }

    /// <summary>
    /// 清理容器
    /// </summary>
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
        Console.WriteLine("SQL Server 容器已清理");
    }
}

/// <summary>
/// Collection Definition，用於共享 SQL Server 容器
/// </summary>
[CollectionDefinition(nameof(SqlServerCollectionFixture))]
public class SqlServerCollectionFixture : ICollectionFixture<SqlServerContainerFixture>
{
    // 此類別只是用來定義 Collection，不需要實作內容
}