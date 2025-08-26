using System.Text.Json;

namespace Day17.Core.Tests;

/// <summary>
/// ConfigManagerService 的單元測試
/// </summary>
public class ConfigManagerServiceTests
{
    [Fact]
    public void InitializeConfigDirectory_當目錄不存在_應建立配置目錄()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        service.InitializeConfigDirectory();

        // Assert
        mockFileSystem.Directory.Exists(configDirectory).Should().BeTrue();
    }

    [Fact]
    public void InitializeConfigDirectory_當目錄已存在_應不影響現有目錄()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";

        mockFileSystem.AddDirectory(configDirectory);
        mockFileSystem.AddFile($@"{configDirectory}\existing.txt", new MockFileData("existing file"));

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        service.InitializeConfigDirectory();

        // Assert
        mockFileSystem.Directory.Exists(configDirectory).Should().BeTrue();
        mockFileSystem.File.Exists($@"{configDirectory}\existing.txt").Should().BeTrue();
    }

    [Fact]
    public async Task LoadAppSettingsAsync_當配置檔案存在_應載入設定()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";
        var settingsPath = $@"{configDirectory}\appsettings.json";

        var existingSettings = new ConfigManagerService.AppSettings
        {
            ApplicationName = "測試應用程式",
            Version = "2.0.0",
            Database = new ConfigManagerService.DatabaseSettings
            {
                ConnectionString = "Server=test;Database=TestDb;",
                TimeoutSeconds = 60
            },
            Logging = new ConfigManagerService.LoggingSettings
            {
                Level = "Debug",
                EnableFileLogging = false,
                LogDirectory = "custom_logs"
            }
        };

        var jsonContent = JsonSerializer.Serialize(existingSettings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        mockFileSystem.AddFile(settingsPath, new MockFileData(jsonContent));

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        var loadedSettings = await service.LoadAppSettingsAsync();

        // Assert
        loadedSettings.Should().NotBeNull();
        loadedSettings.ApplicationName.Should().Be("測試應用程式");
        loadedSettings.Version.Should().Be("2.0.0");
        loadedSettings.Database.ConnectionString.Should().Be("Server=test;Database=TestDb;");
        loadedSettings.Database.TimeoutSeconds.Should().Be(60);
        loadedSettings.Logging.Level.Should().Be("Debug");
        loadedSettings.Logging.EnableFileLogging.Should().BeFalse();
        loadedSettings.Logging.LogDirectory.Should().Be("custom_logs");
    }

    [Fact]
    public async Task LoadAppSettingsAsync_當配置檔案不存在_應建立預設設定並回傳()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        var settings = await service.LoadAppSettingsAsync();

        // Assert
        settings.Should().NotBeNull();
        settings.ApplicationName.Should().Be("Day17 FileSystem Testing Demo");
        settings.Version.Should().Be("1.0.0");
        settings.Database.Should().NotBeNull();
        settings.Logging.Should().NotBeNull();

        // 驗證預設設定檔案已被建立
        const string settingsPath = $@"{configDirectory}\appsettings.json";
        mockFileSystem.File.Exists(settingsPath).Should().BeTrue();
    }

    [Fact]
    public async Task LoadAppSettingsAsync_當JSON格式錯誤_應回傳預設設定()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";
        const string settingsPath = $@"{configDirectory}\appsettings.json";

        mockFileSystem.AddFile(settingsPath, new MockFileData("{ invalid json content"));

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        var settings = await service.LoadAppSettingsAsync();

        // Assert
        settings.Should().NotBeNull();
        settings.ApplicationName.Should().Be("Day17 FileSystem Testing Demo");
        settings.Version.Should().Be("1.0.0");
    }

    [Fact]
    public async Task SaveAppSettingsAsync_應正確序列化並保存設定()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";

        var settings = new ConfigManagerService.AppSettings
        {
            ApplicationName = "自訂應用程式",
            Version = "3.0.0",
            Database = new ConfigManagerService.DatabaseSettings
            {
                ConnectionString = "Server=prod;Database=ProdDb;",
                TimeoutSeconds = 120
            },
            Logging = new ConfigManagerService.LoggingSettings
            {
                Level = "Warning",
                EnableFileLogging = true,
                LogDirectory = "production_logs"
            }
        };

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        await service.SaveAppSettingsAsync(settings);

        // Assert
        const string settingsPath = $@"{configDirectory}\appsettings.json";
        mockFileSystem.File.Exists(settingsPath).Should().BeTrue();
        mockFileSystem.Directory.Exists(configDirectory).Should().BeTrue();

        var savedContent = mockFileSystem.File.ReadAllText(settingsPath);
        var deserializedSettings = JsonSerializer.Deserialize<ConfigManagerService.AppSettings>(savedContent);

        deserializedSettings.Should().NotBeNull();
        deserializedSettings.ApplicationName.Should().Be("自訂應用程式");
        deserializedSettings.Version.Should().Be("3.0.0");
        deserializedSettings.Database.ConnectionString.Should().Be("Server=prod;Database=ProdDb;");
        deserializedSettings.Database.TimeoutSeconds.Should().Be(120);
    }

    [Fact]
    public void BackupConfiguration_當配置檔案存在_應建立備份檔案()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";
        const string settingsPath = $@"{configDirectory}\appsettings.json";
        const string originalContent = """{"ApplicationName":"Test App","Version":"1.0.0"}""";

        mockFileSystem.AddFile(settingsPath, new MockFileData(originalContent));

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        var backupPath = service.BackupConfiguration();

        // Assert
        backupPath.Should().NotBeNullOrEmpty();
        backupPath.Should().StartWith($@"{configDirectory}\backup\appsettings_");
        backupPath.Should().EndWith(".json");

        mockFileSystem.File.Exists(backupPath).Should().BeTrue();
        mockFileSystem.File.ReadAllText(backupPath).Should().Be(originalContent);

        // 驗證備份目錄已建立
        mockFileSystem.Directory.Exists($@"{configDirectory}\backup").Should().BeTrue();
    }

    [Fact]
    public void BackupConfiguration_當配置檔案不存在_應拋出FileNotFoundException()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act & Assert
        var action = () => service.BackupConfiguration();
        action.Should().Throw<FileNotFoundException>()
              .WithMessage("找不到要備份的配置檔案");
    }

    [Fact]
    public void GetAvailableBackups_當有多個備份檔案_應回傳按時間排序的清單()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";
        const string backupDirectory = $@"{configDirectory}\backup";

        var baseTime = new DateTime(2024, 1, 15, 10, 0, 0);
        var backups = new[]
        {
            ($@"{backupDirectory}\appsettings_20240115_100000.json", baseTime, "backup1"),
            ($@"{backupDirectory}\appsettings_20240115_110000.json", baseTime.AddHours(1), "backup2"),
            ($@"{backupDirectory}\appsettings_20240115_120000.json", baseTime.AddHours(2), "backup3")
        };

        foreach (var (path, creationTime, content) in backups)
        {
            mockFileSystem.AddFile(path, new MockFileData(content)
            {
                CreationTime = creationTime
            });
        }

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        var availableBackups = service.GetAvailableBackups();

        // Assert
        availableBackups.Should().HaveCount(3);
        availableBackups.Should().BeInDescendingOrder(b => b.CreationTime);

        // 驗證最新的備份在最前面
        availableBackups[0].FileName.Should().Be("appsettings_20240115_120000.json");
        availableBackups[1].FileName.Should().Be("appsettings_20240115_110000.json");
        availableBackups[2].FileName.Should().Be("appsettings_20240115_100000.json");

        // 驗證檔案資訊正確
        foreach (var backup in availableBackups)
        {
            backup.FilePath.Should().NotBeNullOrEmpty();
            backup.Size.Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public void GetAvailableBackups_當備份目錄不存在_應回傳空清單()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        var availableBackups = service.GetAvailableBackups();

        // Assert
        availableBackups.Should().BeEmpty();
    }

    [Fact]
    public void GetAvailableBackups_當備份目錄為空_應回傳空清單()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";
        const string backupDirectory = $@"{configDirectory}\backup";

        mockFileSystem.AddDirectory(backupDirectory);

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        var availableBackups = service.GetAvailableBackups();

        // Assert
        availableBackups.Should().BeEmpty();
    }

    [Fact]
    public void RestoreFromBackup_當備份檔案存在_應還原配置()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";
        const string settingsPath = $@"{configDirectory}\appsettings.json";
        const string backupPath = $@"{configDirectory}\backup\appsettings_20240115_100000.json";

        const string currentContent = """{"ApplicationName":"Current","Version":"1.0.0"}""";
        const string backupContent = """{"ApplicationName":"Backup","Version":"0.9.0"}""";

        mockFileSystem.AddFile(settingsPath, new MockFileData(currentContent));
        mockFileSystem.AddFile(backupPath, new MockFileData(backupContent));

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act
        service.RestoreFromBackup(backupPath);

        // Assert
        mockFileSystem.File.ReadAllText(settingsPath).Should().Be(backupContent);

        // 驗證備份檔案仍然存在
        mockFileSystem.File.Exists(backupPath).Should().BeTrue();
    }

    [Fact]
    public void RestoreFromBackup_當備份檔案不存在_應拋出FileNotFoundException()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";
        var nonexistentBackup = $@"{configDirectory}\backup\missing.json";

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act & Assert
        var action = () => service.RestoreFromBackup(nonexistentBackup);
        action.Should().Throw<FileNotFoundException>()
              .WithMessage($"備份檔案不存在: {nonexistentBackup}");
    }

    [Fact]
    public async Task 整合測試_完整的配置管理流程()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string configDirectory = @"C:\app\config";

        var service = new ConfigManagerService(mockFileSystem, configDirectory);

        // Act & Assert - 載入預設設定
        var initialSettings = await service.LoadAppSettingsAsync();
        initialSettings.ApplicationName.Should().Be("Day17 FileSystem Testing Demo");

        // 修改設定
        initialSettings.ApplicationName = "整合測試應用程式";
        initialSettings.Version = "2.0.0";
        await service.SaveAppSettingsAsync(initialSettings);

        // 建立備份
        var backupPath = service.BackupConfiguration();
        backupPath.Should().NotBeNullOrEmpty();

        // 再次修改設定
        initialSettings.ApplicationName = "修改後的應用程式";
        await service.SaveAppSettingsAsync(initialSettings);

        // 驗證修改已保存
        var modifiedSettings = await service.LoadAppSettingsAsync();
        modifiedSettings.ApplicationName.Should().Be("修改後的應用程式");

        // 從備份還原
        service.RestoreFromBackup(backupPath);

        // 驗證已還原到備份版本
        var restoredSettings = await service.LoadAppSettingsAsync();
        restoredSettings.ApplicationName.Should().Be("整合測試應用程式");
        restoredSettings.Version.Should().Be("2.0.0");

        // 驗證備份清單
        var backups = service.GetAvailableBackups();
        backups.Should().HaveCount(1);
        backups[0].FilePath.Should().Be(backupPath);
    }

    [Theory]
    [InlineData("custom_config")]
    [InlineData(@"C:\MyApp\Config")]
    public void ConfigManagerService_使用不同配置目錄_應正常運作(string configDir)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var service = new ConfigManagerService(mockFileSystem, configDir);

        // Act
        service.InitializeConfigDirectory();

        // Assert
        mockFileSystem.Directory.Exists(configDir).Should().BeTrue();
    }

    [Fact]
    public void ConfigManagerService_使用空配置目錄_應使用預設目錄()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var initialDirectories = mockFileSystem.AllDirectories.ToList();
        var service = new ConfigManagerService(mockFileSystem, "");

        // Act
        service.InitializeConfigDirectory();

        // Assert
        // 空目錄不應建立新的目錄，目錄數量應保持不變
        mockFileSystem.AllDirectories.Should().HaveCount(initialDirectories.Count);
    }
}