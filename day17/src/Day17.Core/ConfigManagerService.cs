using System.Text.Json;

namespace Day17.Core;

/// <summary>
/// 整合配置管理器，示範實際應用場景
/// </summary>
public class ConfigManagerService
{
    private readonly IFileSystem _fileSystem;
    private readonly string _configDirectory;

    public ConfigManagerService(IFileSystem fileSystem, string configDirectory = "config")
    {
        this._fileSystem = fileSystem;
        this._configDirectory = configDirectory;
    }

    /// <summary>
    /// 初始化配置目錄
    /// </summary>
    public void InitializeConfigDirectory()
    {
        if (!string.IsNullOrWhiteSpace(this._configDirectory) && !this._fileSystem.Directory.Exists(this._configDirectory))
        {
            this._fileSystem.Directory.CreateDirectory(this._configDirectory);
        }
    }

    /// <summary>
    /// 載入應用程式設定
    /// </summary>
    /// <returns>應用程式設定</returns>
    public async Task<AppSettings> LoadAppSettingsAsync()
    {
        var configPath = this._fileSystem.Path.Combine(this._configDirectory, "appsettings.json");

        if (!this._fileSystem.File.Exists(configPath))
        {
            var defaultSettings = new AppSettings();
            await this.SaveAppSettingsAsync(defaultSettings);
            return defaultSettings;
        }

        try
        {
            var jsonContent = await this._fileSystem.File.ReadAllTextAsync(configPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(jsonContent);
            return settings ?? new AppSettings();
        }
        catch (Exception)
        {
            return new AppSettings();
        }
    }

    /// <summary>
    /// 保存應用程式設定
    /// </summary>
    /// <param name="settings">應用程式設定</param>
    public async Task SaveAppSettingsAsync(AppSettings settings)
    {
        this.InitializeConfigDirectory();

        var configPath = this._fileSystem.Path.Combine(this._configDirectory, "appsettings.json");
        var jsonContent = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await this._fileSystem.File.WriteAllTextAsync(configPath, jsonContent);
    }

    /// <summary>
    /// 備份現有配置
    /// </summary>
    /// <returns>備份檔案路徑</returns>
    public string BackupConfiguration()
    {
        var configPath = this._fileSystem.Path.Combine(this._configDirectory, "appsettings.json");

        if (!this._fileSystem.File.Exists(configPath))
        {
            throw new FileNotFoundException("找不到要備份的配置檔案");
        }

        var backupDirectory = this._fileSystem.Path.Combine(this._configDirectory, "backup");
        if (!this._fileSystem.Directory.Exists(backupDirectory))
        {
            this._fileSystem.Directory.CreateDirectory(backupDirectory);
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupFileName = $"appsettings_{timestamp}.json";
        var backupPath = this._fileSystem.Path.Combine(backupDirectory, backupFileName);

        this._fileSystem.File.Copy(configPath, backupPath);
        return backupPath;
    }

    /// <summary>
    /// 取得所有可用的備份檔案
    /// </summary>
    /// <returns>備份檔案清單</returns>
    public List<BackupInfo> GetAvailableBackups()
    {
        var backupDirectory = this._fileSystem.Path.Combine(this._configDirectory, "backup");

        if (!this._fileSystem.Directory.Exists(backupDirectory))
        {
            return new List<BackupInfo>();
        }

        return this._fileSystem.Directory.GetFiles(backupDirectory, "appsettings_*.json")
                   .Select(filePath => new BackupInfo
                   {
                       FilePath = filePath,
                       FileName = this._fileSystem.Path.GetFileName(filePath),
                       CreationTime = this._fileSystem.File.GetCreationTime(filePath),
                       Size = this._fileSystem.FileInfo.New(filePath).Length
                   })
                   .OrderByDescending(b => b.CreationTime)
                   .ToList();
    }

    /// <summary>
    /// 從備份還原配置
    /// </summary>
    /// <param name="backupFilePath">備份檔案路徑</param>
    public void RestoreFromBackup(string backupFilePath)
    {
        if (!this._fileSystem.File.Exists(backupFilePath))
        {
            throw new FileNotFoundException($"備份檔案不存在: {backupFilePath}");
        }

        var configPath = this._fileSystem.Path.Combine(this._configDirectory, "appsettings.json");
        this._fileSystem.File.Copy(backupFilePath, configPath, overwrite: true);
    }

    /// <summary>
    /// 應用程式設定
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// 應用程式名稱
        /// </summary>
        public string ApplicationName { get; set; } = "Day17 FileSystem Testing Demo";

        /// <summary>
        /// 應用程式版本
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// 資料庫設定
        /// </summary>
        public DatabaseSettings Database { get; set; } = new();

        /// <summary>
        /// 日誌設定
        /// </summary>
        public LoggingSettings Logging { get; set; } = new();
    }

    /// <summary>
    /// 資料庫設定
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// 資料庫連接字串
        /// </summary>
        public string ConnectionString { get; set; } = "Server=localhost;Database=TestDb;";

        /// <summary>
        /// 連接逾時秒數
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }

    /// <summary>
    /// 日誌設定
    /// </summary>
    public class LoggingSettings
    {
        /// <summary>
        /// 日誌等級 (例如: Information, Warning, Error)
        /// </summary>
        public string Level { get; set; } = "Information";

        /// <summary>
        /// 是否啟用檔案日誌
        /// </summary>
        public bool EnableFileLogging { get; set; } = true;

        /// <summary>
        /// 日誌目錄
        /// </summary>
        public string LogDirectory { get; set; } = "logs";
    }

    /// <summary>
    /// 備份資訊
    /// </summary>
    public class BackupInfo
    {
        /// <summary>
        /// 備份檔案完整路徑
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 備份檔案名稱
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 備份檔案建立時間
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 備份檔案大小 (位元組)
        /// </summary>
        public long Size { get; set; }
    }
}