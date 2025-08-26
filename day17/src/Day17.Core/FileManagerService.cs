namespace Day17.Core;

/// <summary>
/// 檔案管理服務
/// </summary>
public class FileManagerService
{
    private readonly IFileSystem _fileSystem;

    public FileManagerService(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
    }

    /// <summary>
    /// 複製檔案到指定目錄
    /// </summary>
    /// <param name="sourceFilePath">來源檔案路徑</param>
    /// <param name="targetDirectory">目標目錄</param>
    /// <returns>目標檔案路徑</returns>
    public string CopyFileToDirectory(string sourceFilePath, string targetDirectory)
    {
        if (!this._fileSystem.File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException($"來源檔案不存在: {sourceFilePath}");
        }

        if (!this._fileSystem.Directory.Exists(targetDirectory))
        {
            this._fileSystem.Directory.CreateDirectory(targetDirectory);
        }

        var fileName = this._fileSystem.Path.GetFileName(sourceFilePath);
        var targetFilePath = this._fileSystem.Path.Combine(targetDirectory, fileName);

        this._fileSystem.File.Copy(sourceFilePath, targetFilePath, overwrite: true);
        return targetFilePath;
    }

    /// <summary>
    /// 備份檔案（加上時間戳記）
    /// </summary>
    /// <param name="filePath">要備份的檔案路徑</param>
    /// <returns>備份檔案路徑</returns>
    public string BackupFile(string filePath)
    {
        if (!this._fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException($"檔案不存在: {filePath}");
        }

        var directory = this._fileSystem.Path.GetDirectoryName(filePath);
        var fileNameWithoutExtension = this._fileSystem.Path.GetFileNameWithoutExtension(filePath);
        var extension = this._fileSystem.Path.GetExtension(filePath);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        var backupFileName = $"{fileNameWithoutExtension}_{timestamp}{extension}";
        var backupFilePath = this._fileSystem.Path.Combine(directory ?? "", backupFileName);

        this._fileSystem.File.Copy(filePath, backupFilePath);
        return backupFilePath;
    }

    /// <summary>
    /// 清理舊備份檔案（保留指定數量的最新檔案）
    /// </summary>
    /// <param name="directory">備份目錄</param>
    /// <param name="pattern">檔案模式</param>
    /// <param name="keepCount">保留的檔案數量</param>
    /// <returns>刪除的檔案數量</returns>
    public int CleanupOldBackups(string directory, string pattern, int keepCount)
    {
        if (!this._fileSystem.Directory.Exists(directory))
        {
            return 0;
        }

        var files = this._fileSystem.Directory.GetFiles(directory, pattern)
                        .Select(f => new
                        {
                            Path = f,
                            CreationTime = this._fileSystem.File.GetCreationTime(f)
                        })
                        .OrderByDescending(f => f.CreationTime)
                        .Skip(keepCount)
                        .ToList();

        foreach (var file in files)
        {
            this._fileSystem.File.Delete(file.Path);
        }

        return files.Count;
    }

    /// <summary>
    /// 取得檔案資訊
    /// </summary>
    /// <param name="filePath">檔案路徑</param>
    /// <returns>檔案資訊</returns>
    public FileInfoData? GetFileInfo(string filePath)
    {
        if (!this._fileSystem.File.Exists(filePath))
        {
            return null;
        }

        var fileInfo = this._fileSystem.FileInfo.New(filePath);
        return new FileInfoData
        {
            Name = fileInfo.Name,
            FullPath = fileInfo.FullName,
            Size = fileInfo.Length,
            CreationTime = fileInfo.CreationTime,
            LastWriteTime = fileInfo.LastWriteTime,
            IsReadOnly = fileInfo.IsReadOnly
        };
    }

    /// <summary>
    /// 檔案資訊資料類別
    /// </summary>
    public class FileInfoData
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 完整路徑
        /// </summary>
        public string FullPath { get; set; } = string.Empty;

        /// <summary>
        /// 檔案大小（位元組）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// 是否唯讀
        /// </summary>
        public bool IsReadOnly { get; set; }
    }
}