using Microsoft.Data.Sqlite;

namespace Calculator.Tests.V3.Fixtures;

/// <summary>
/// Assembly Fixture 展示：整個測試組件級別的共享資源管理
/// 這會在整個測試執行開始時初始化一次，結束時清理一次
/// </summary>
public class DatabaseAssemblyFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public IConfiguration Configuration { get; private set; } = null!;
    public string ConnectionString { get; private set; } = null!;

    private SqliteConnection? _connection;
    private readonly List<string> _createdTables = new();

    /// <summary>
    /// Assembly 級別初始化：在所有測試執行前執行一次
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        Console.WriteLine("DatabaseAssemblyFixture: 開始初始化 Assembly 層級資源");

        // 設定配置
        var configBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection([
                new KeyValuePair<string, string?>("ConnectionStrings:TestDatabase", "Data Source=:memory:"),
                new KeyValuePair<string, string?>("TestSettings:Environment", "Test"),
                new KeyValuePair<string, string?>("TestSettings:LogLevel", "Debug"),
                new KeyValuePair<string, string?>("TestSettings:EnableDetailedLogging", "true")
            ]);

        Configuration = configBuilder.Build();
        ConnectionString = Configuration.GetConnectionString("TestDatabase") ?? "";

        // 設定依賴注入容器
        var services = new ServiceCollection();

        services.AddSingleton(Configuration);
        services.AddScoped<Core.Calculator>();
        services.AddLogging();

        ServiceProvider = services.BuildServiceProvider();

        // 初始化內存 SQLite 資料庫
        await InitializeTestDatabaseAsync();

        Console.WriteLine("DatabaseAssemblyFixture: Assembly 層級資源初始化完成");
    }

    /// <summary>
    /// Assembly 級別清理：在所有測試執行後執行一次
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("DatabaseAssemblyFixture: 開始清理 Assembly 層級資源");

        // 清理資料庫
        await CleanupTestDatabaseAsync();

        // 釋放連線
        _connection?.Close();
        _connection?.Dispose();

        // 釋放服務提供者
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        Console.WriteLine("DatabaseAssemblyFixture: Assembly 層級資源清理完成");
    }

    /// <summary>
    /// 初始化測試資料庫
    /// </summary>
    private async Task InitializeTestDatabaseAsync()
    {
        // 建立 SQLite 內存資料庫連線
        _connection = new SqliteConnection(ConnectionString);
        await _connection.OpenAsync();

        // 建立測試表格
        await CreateTestTablesAsync();

        // 插入測試資料
        await SeedTestDataAsync();
    }

    /// <summary>
    /// 建立測試表格
    /// </summary>
    private async Task CreateTestTablesAsync()
    {
        const string createUserTable = """
                                       CREATE TABLE Users (
                                           Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                           Name TEXT NOT NULL,
                                           Email TEXT UNIQUE NOT NULL,
                                           CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                                       )
                                       """;

        const string createCalculationLogTable = """
                                                 CREATE TABLE CalculationLogs (
                                                     Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                     Operation TEXT NOT NULL,
                                                     OperandA REAL NOT NULL,
                                                     OperandB REAL NOT NULL,
                                                     Result REAL NOT NULL,
                                                     Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                                                 )
                                                 """;

        await using var command = _connection!.CreateCommand();

        command.CommandText = createUserTable;
        await command.ExecuteNonQueryAsync();
        _createdTables.Add("Users");

        command.CommandText = createCalculationLogTable;
        await command.ExecuteNonQueryAsync();
        _createdTables.Add("CalculationLogs");
    }

    /// <summary>
    /// 植入測試資料
    /// </summary>
    private async Task SeedTestDataAsync()
    {
        const string insertUsers = """
                                   INSERT INTO Users (Name, Email) VALUES
                                   ('測試使用者1', 'test1@example.com'),
                                   ('測試使用者2', 'test2@example.com'),
                                   ('Administrator', 'admin@example.com')
                                   """;

        await using var command = _connection!.CreateCommand();
        command.CommandText = insertUsers;
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// 清理測試資料庫
    /// </summary>
    private async Task CleanupTestDatabaseAsync()
    {
        if (_connection == null)
        {
            return;
        }

        try
        {
            // 清空所有表格資料
            foreach (var tableName in _createdTables)
            {
                await using var command = _connection.CreateCommand();
                command.CommandText = $"DELETE FROM {tableName}";
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"清理資料庫時發生錯誤: {ex.Message}");
        }
    }

    /// <summary>
    /// 取得特定類型的服務
    /// </summary>
    public T GetService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// 建立服務範圍
    /// </summary>
    public IServiceScope CreateScope()
    {
        return ServiceProvider.CreateScope();
    }

    /// <summary>
    /// 取得資料庫連線
    /// </summary>
    public SqliteConnection GetConnection()
    {
        return _connection ?? throw new InvalidOperationException("資料庫連線尚未初始化");
    }

    /// <summary>
    /// 執行 SQL 查詢並回傳結果
    /// </summary>
    public async Task<List<T>> QueryAsync<T>(string sql, Func<SqliteDataReader, T> mapper)
    {
        var results = new List<T>();

        await using var command = _connection!.CreateCommand();
        command.CommandText = sql;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(mapper(reader));
        }

        return results;
    }

    /// <summary>
    /// 記錄計算操作到資料庫
    /// </summary>
    public async Task LogCalculationAsync(string operation, double operandA, double operandB, double result)
    {
        const string sql = """
                           INSERT INTO CalculationLogs (Operation, OperandA, OperandB, Result)
                           VALUES (@operation, @operandA, @operandB, @result)
                           """;

        await using var command = _connection!.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@operation", operation);
        command.Parameters.AddWithValue("@operandA", operandA);
        command.Parameters.AddWithValue("@operandB", operandB);
        command.Parameters.AddWithValue("@result", result);

        await command.ExecuteNonQueryAsync();
    }
}