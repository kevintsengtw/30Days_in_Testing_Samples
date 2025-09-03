namespace Day25.Tests.Integration.Infrastructure;

/// <summary>
/// 資料庫管理員 - 使用 Aspire 提供的連線字串
/// </summary>
public class DatabaseManager
{
    private readonly Func<Task<string>> _getConnectionStringAsync;
    private Respawner? _respawner;

    public DatabaseManager(Func<Task<string>> getConnectionStringAsync)
    {
        _getConnectionStringAsync = getConnectionStringAsync;
    }

    /// <summary>
    /// 初始化資料庫結構
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        var connectionString = await _getConnectionStringAsync();

        // 首先確保 productdb 資料庫存在
        await EnsureDatabaseExistsAsync(connectionString);

        // 連線到 productdb 並確保資料表存在
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // 確保資料表存在
        await EnsureTablesExistAsync(connection);

        // 初始化 Respawner
        if (_respawner == null)
        {
            _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                TablesToIgnore = new Table[] { "__EFMigrationsHistory" },
                SchemasToInclude = new[] { "public" },
                DbAdapter = DbAdapter.Postgres
            });
        }
    }

    /// <summary>
    /// 清理測試資料
    /// </summary>
    public async Task CleanDatabaseAsync()
    {
        if (_respawner == null)
        {
            return;
        }

        var connectionString = await _getConnectionStringAsync();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    /// <summary>
    /// 取得連線字串
    /// </summary>
    public async Task<string> GetConnectionStringAsync()
    {
        return await _getConnectionStringAsync();
    }

    /// <summary>
    /// 確保資料庫存在 - 此時 PostgreSQL 服務應該已經就緒
    /// </summary>
    private async Task EnsureDatabaseExistsAsync(string connectionString)
    {
        // 解析連線字串取得伺服器資訊
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;

        // 連線到 postgres 預設資料庫檢查並創建 productdb
        builder.Database = "postgres";
        var masterConnectionString = builder.ToString();

        await using var connection = new NpgsqlConnection(masterConnectionString);
        await connection.OpenAsync();

        // 檢查資料庫是否已存在
        var checkDbQuery = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
        await using var checkCommand = new NpgsqlCommand(checkDbQuery, connection);
        var dbExists = await checkCommand.ExecuteScalarAsync();

        if (dbExists == null)
        {
            // 建立 productdb 資料庫
            var createDbQuery = $"CREATE DATABASE \"{databaseName}\"";
            await using var createCommand = new NpgsqlCommand(createDbQuery, connection);
            await createCommand.ExecuteNonQueryAsync();
            Console.WriteLine($"Database has been created: {databaseName}");
        }
        else
        {
            Console.WriteLine($"The database already exists: {databaseName}");
        }
    }

    /// <summary>
    /// 確保必要的資料表存在
    /// </summary>
    private async Task EnsureTablesExistAsync(NpgsqlConnection connection)
    {
        const string createProductTableSql = """
                                             CREATE TABLE IF NOT EXISTS products (
                                                 id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                                 name VARCHAR(100) NOT NULL,
                                                 price DECIMAL(10,2) NOT NULL,
                                                 created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                                                 updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
                                             );
                                             """;

        await using var command = new NpgsqlCommand(createProductTableSql, connection);
        await command.ExecuteNonQueryAsync();
        Console.WriteLine("Ensure that the `products` table exists");
    }
}