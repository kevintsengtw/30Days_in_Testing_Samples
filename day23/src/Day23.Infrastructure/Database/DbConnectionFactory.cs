using Microsoft.Extensions.Configuration;

namespace Day23.Infrastructure.Database;

/// <summary>
/// 資料庫連線工廠
/// </summary>
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection 連線字串不能為空");
    }

    /// <summary>
    /// 建立資料庫連線
    /// </summary>
    public NpgsqlConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}