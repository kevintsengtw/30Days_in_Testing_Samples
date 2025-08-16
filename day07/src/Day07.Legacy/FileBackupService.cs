using Microsoft.Data.SqlClient;

namespace Day07.Legacy;

/// <summary>
/// 注意！這是一個教學範例：刻意違反 SRP 原則來展示問題
/// 實際專案中請避免這樣的設計，應遵循 SOLID 原則
/// </summary>
public class FileBackupService
{
    /// <summary>
    /// 備份檔案到指定路徑
    /// </summary>
    /// <param name="sourcePath">來源路徑</param>
    /// <param name="destinationPath">目標路徑</param>
    /// <returns></returns>
    public BackupResult BackupFile(string sourcePath, string destinationPath)
    {
        try
        {
            // 直接依賴檔案系統
            if (!File.Exists(sourcePath))
            {
                return new BackupResult { Success = false, Message = "Source file not found" };
            }

            var fileInfo = new FileInfo(sourcePath);
            if (fileInfo.Length > 100 * 1024 * 1024) // 100MB
            {
                return new BackupResult { Success = false, Message = "File too large" };
            }

            // 直接依賴時間
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{Path.GetFileNameWithoutExtension(sourcePath)}_{timestamp}{Path.GetExtension(sourcePath)}";
            var fullBackupPath = Path.Combine(destinationPath, backupFileName);

            // 執行備份
            File.Copy(sourcePath, fullBackupPath);

            // 直接依賴資料庫
            using var connection = new SqlConnection("Data Source=.;Initial Catalog=BackupDB;Integrated Security=true");
            connection.Open();
            var command = new SqlCommand(
                "INSERT INTO BackupHistory (SourcePath, BackupPath, BackupTime) VALUES (@source, @backup, @time)",
                connection);
            command.Parameters.AddWithValue("@source", sourcePath);
            command.Parameters.AddWithValue("@backup", fullBackupPath);
            command.Parameters.AddWithValue("@time", DateTime.Now);
            command.ExecuteNonQuery();

            return new BackupResult { Success = true, BackupPath = fullBackupPath };
        }
        catch (Exception ex)
        {
            // 直接使用 Console.WriteLine（無法測試記錄行為）
            Console.WriteLine($"Backup failed: {ex.Message}");
            return new BackupResult { Success = false, Message = ex.Message };
        }
    }
}