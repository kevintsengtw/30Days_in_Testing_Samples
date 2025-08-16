using Day07.Refactored.Abstractions;

namespace Day07.Refactored;

/// <summary>
/// class FileBackupService - 檔案備份服務
/// </summary>
public class FileBackupService
{
    private readonly IFileSystem _fileSystem;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBackupRepository _backupRepository;
    private readonly ILogger<FileBackupService> _logger;

    /// <summary>
    /// 檔案備份服務建構子
    /// </summary>
    /// <param name="fileSystem">檔案系統</param>
    /// <param name="dateTimeProvider">日期時間提供者</param>
    /// <param name="backupRepository">備份資料庫</param>
    /// <param name="logger">日誌記錄器</param>
    public FileBackupService(IFileSystem fileSystem,
                             IDateTimeProvider dateTimeProvider,
                             IBackupRepository backupRepository,
                             ILogger<FileBackupService> logger)
    {
        this._fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this._dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        this._backupRepository = backupRepository ?? throw new ArgumentNullException(nameof(backupRepository));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 備份檔案到指定路徑
    /// </summary>
    /// <param name="sourcePath">來源路徑</param>
    /// <param name="destinationPath">目標路徑</param>
    /// <returns></returns>
    public async Task<BackupResult> BackupFileAsync(string sourcePath, string destinationPath)
    {
        try
        {
            this._logger.LogInformation("Starting backup from {SourcePath} to {DestinationPath}",
                                        sourcePath, destinationPath);

            if (!this._fileSystem.FileExists(sourcePath))
            {
                const string message = "Source file not found";
                this._logger.LogWarning("Backup failed: {Message}. Source: {SourcePath}", message, sourcePath);
                return new BackupResult { Success = false, Message = message };
            }

            var fileInfo = this._fileSystem.GetFileInfo(sourcePath);
            if (fileInfo.Length > 100 * 1024 * 1024) // 100MB
            {
                const string message = "File too large";
                this._logger.LogWarning("Backup failed: {Message}. File size: {Size} bytes", message, fileInfo.Length);
                return new BackupResult { Success = false, Message = message };
            }

            var timestamp = this._dateTimeProvider.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{Path.GetFileNameWithoutExtension(sourcePath)}_{timestamp}{Path.GetExtension(sourcePath)}";
            var fullBackupPath = Path.Combine(destinationPath, backupFileName);

            this._fileSystem.CopyFile(sourcePath, fullBackupPath);

            await this._backupRepository.SaveBackupHistoryAsync(sourcePath, fullBackupPath, this._dateTimeProvider.Now);

            this._logger.LogInformation("Backup completed successfully. Backup path: {BackupPath}", fullBackupPath);

            return new BackupResult { Success = true, BackupPath = fullBackupPath };
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Backup failed for {SourcePath}", sourcePath);
            return new BackupResult { Success = false, Message = ex.Message };
        }
    }
}