namespace Day07.Refactored.Abstractions;

/// <summary>
/// interface IBackupRepository - 備份資料庫介面
/// </summary>
public interface IBackupRepository
{
    /// <summary>
    /// 儲存備份歷史
    /// </summary>
    /// <param name="sourcePath">來源路徑</param>
    /// <param name="backupPath">備份路徑</param>
    /// <param name="backupTime">備份時間</param>
    Task SaveBackupHistoryAsync(string sourcePath, string backupPath, DateTime backupTime);
}