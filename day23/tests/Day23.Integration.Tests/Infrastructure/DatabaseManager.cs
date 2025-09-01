using Dapper;
using Npgsql;

namespace Day23.Integration.Tests.Infrastructure;

/// <summary>
/// 資料庫管理工具
/// </summary>
public class DatabaseManager
{
    private readonly string _connectionString;
    private Respawner? _respawner;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// 初始化資料庫結構
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // 使用 SQL 指令碼外部化策略建立資料表
        await EnsureTablesExistAsync(connection);

        // 初始化 Respawner
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres
        });
    }

    /// <summary>
    /// 確保資料表存在，使用外部 SQL 指令碼建立
    /// 實作第 Day 21 介紹的 SQL 指令碼外部化策略
    /// </summary>
    private async Task EnsureTablesExistAsync(NpgsqlConnection connection)
    {
        var scriptDirectory = Path.Combine(AppContext.BaseDirectory, "SqlScripts");
        if (!Directory.Exists(scriptDirectory))
        {
            throw new DirectoryNotFoundException($"SQL 指令碼目錄不存在: {scriptDirectory}");
        }

        // 按照依賴順序執行表格建立腳本
        var orderedScripts = new[]
        {
            "Tables/CreateProductsTable.sql"
        };

        foreach (var scriptPath in orderedScripts)
        {
            var fullPath = Path.Combine(scriptDirectory, scriptPath);
            if (File.Exists(fullPath))
            {
                var script = await File.ReadAllTextAsync(fullPath);
                await using var command = new NpgsqlCommand(script, connection);
                await command.ExecuteNonQueryAsync();
            }
            else
            {
                throw new FileNotFoundException($"SQL 指令碼檔案不存在: {fullPath}");
            }
        }
    }

    /// <summary>
    /// 清理資料庫資料
    /// </summary>
    public async Task CleanDatabaseAsync()
    {
        if (_respawner == null)
        {
            throw new InvalidOperationException("Respawner 尚未初始化，請先呼叫 InitializeDatabaseAsync");
        }

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    /// <summary>
    /// 插入測試產品資料
    /// </summary>
    public async Task<Product> SeedProductAsync(string name = "測試產品", decimal price = 100.00m)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = """
                  INSERT INTO products (name, price, created_at, updated_at)
                  VALUES (@name, @price, @createdAt, @updatedAt)
                  RETURNING id, name, price, created_at, updated_at;
                  """;

        var now = DateTimeOffset.UtcNow;
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("price", price);
        command.Parameters.AddWithValue("createdAt", now);
        command.Parameters.AddWithValue("updatedAt", now);

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new Product
        {
            Id = reader.GetFieldValue<Guid>(0),
            Name = reader.GetString(1),
            Price = reader.GetDecimal(2),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(3),
            UpdatedAt = reader.GetFieldValue<DateTimeOffset>(4)
        };
    }

    /// <summary>
    /// 執行 SQL 命令（用於測試數據種子）
    /// </summary>
    public async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        return await connection.ExecuteAsync(sql, parameters);
    }
}