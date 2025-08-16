using Day07.Refactored.Abstractions;

namespace Day07.Refactored.Implementations;

/// <summary>
/// class SqlBackupRepository - SQL 備份資料庫
/// </summary>
public class SqlBackupRepository : IBackupRepository
{
    private readonly string _connectionString;

    /// <summary>
    /// SQL 備份資料庫建構子
    /// </summary>
    /// <param name="connectionString">資料庫連接字串</param>
    public SqlBackupRepository(string connectionString)
    {
        this._connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// 儲存備份歷史紀錄
    /// </summary>
    /// <param name="sourcePath">來源路徑</param>
    /// <param name="backupPath">備份路徑</param>
    /// <param name="backupTime">備份時間</param>
    public async Task SaveBackupHistoryAsync(string sourcePath, string backupPath, DateTime backupTime)
    {
        // 實際實作會使用 Entity Framework 或其他 ORM
        // 這裡只是示範如何將資料庫操作抽象化
        await Task.Delay(10); // 模擬非同步資料庫操作

        // 在真實專案中，這裡會有實際的資料庫插入操作
        // using var connection = new SqlConnection(_connectionString);
        // await connection.OpenAsync();
        // var command = new SqlCommand("INSERT INTO BackupHistory...", connection);
        // await command.ExecuteNonQueryAsync();
    }
}