using System.Text;

namespace Day17.Core.Tests;

/// <summary>
/// FileManagerService 的單元測試
/// </summary>
public class FileManagerServiceTests
{
    [Fact]
    public void CopyFileToDirectory_當來源檔案存在_應成功複製到目標目錄()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string sourceFile = @"C:\source\document.txt";
        const string targetDirectory = @"C:\target";
        const string expectedTargetFile = @"C:\target\document.txt";
        const string fileContent = "測試文件內容";

        mockFileSystem.AddFile(sourceFile, new MockFileData(fileContent));

        var service = new FileManagerService(mockFileSystem);

        // Act
        var result = service.CopyFileToDirectory(sourceFile, targetDirectory);

        // Assert
        result.Should().Be(expectedTargetFile);
        mockFileSystem.File.Exists(expectedTargetFile).Should().BeTrue();
        mockFileSystem.File.ReadAllText(expectedTargetFile).Should().Be(fileContent);
        mockFileSystem.Directory.Exists(targetDirectory).Should().BeTrue();
    }

    [Fact]
    public void CopyFileToDirectory_當來源檔案不存在_應拋出FileNotFoundException()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string sourceFile = @"C:\source\nonexistent.txt";
        const string targetDirectory = @"C:\target";

        var service = new FileManagerService(mockFileSystem);

        // Act & Assert
        var action = () => service.CopyFileToDirectory(sourceFile, targetDirectory);
        action.Should().Throw<FileNotFoundException>()
              .WithMessage($"來源檔案不存在: {sourceFile}");
    }

    [Fact]
    public void CopyFileToDirectory_當目標目錄不存在_應建立目錄並複製檔案()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string sourceFile = @"C:\source\document.txt";
        const string targetDirectory = @"C:\new\target\directory";
        const string fileContent = "測試內容";

        mockFileSystem.AddFile(sourceFile, new MockFileData(fileContent));

        var service = new FileManagerService(mockFileSystem);

        // Act
        var result = service.CopyFileToDirectory(sourceFile, targetDirectory);

        // Assert
        mockFileSystem.Directory.Exists(targetDirectory).Should().BeTrue();
        mockFileSystem.File.Exists(result).Should().BeTrue();
    }

    [Fact]
    public void BackupFile_當檔案存在_應建立帶時間戳記的備份檔案()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string originalFile = @"C:\data\important.txt";
        const string fileContent = "重要資料";

        mockFileSystem.AddFile(originalFile, new MockFileData(fileContent));

        var service = new FileManagerService(mockFileSystem);

        // Act
        var backupPath = service.BackupFile(originalFile);

        // Assert
        backupPath.Should().StartWith(@"C:\data\important_");
        backupPath.Should().EndWith(".txt");
        backupPath.Should().NotBe(originalFile);

        mockFileSystem.File.Exists(backupPath).Should().BeTrue();
        mockFileSystem.File.ReadAllText(backupPath).Should().Be(fileContent);

        // 驗證原檔案仍然存在
        mockFileSystem.File.Exists(originalFile).Should().BeTrue();
    }

    [Fact]
    public void BackupFile_當檔案不存在_應拋出FileNotFoundException()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string nonexistentFile = @"C:\data\missing.txt";

        var service = new FileManagerService(mockFileSystem);

        // Act & Assert
        var action = () => service.BackupFile(nonexistentFile);
        action.Should().Throw<FileNotFoundException>()
              .WithMessage($"檔案不存在: {nonexistentFile}");
    }

    [Fact]
    public void CleanupOldBackups_當有多個備份檔案_應保留指定數量的最新檔案()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string backupDirectory = @"C:\backups";
        const string pattern = "backup_*.txt";

        // 建立多個備份檔案，設定不同的建立時間
        var baseTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var backupFiles = new[]
        {
            (@"C:\backups\backup_001.txt", baseTime),
            (@"C:\backups\backup_002.txt", baseTime.AddMinutes(10)),
            (@"C:\backups\backup_003.txt", baseTime.AddMinutes(20)),
            (@"C:\backups\backup_004.txt", baseTime.AddMinutes(30)),
            (@"C:\backups\backup_005.txt", baseTime.AddMinutes(40))
        };

        foreach (var (filePath, creationTime) in backupFiles)
        {
            mockFileSystem.AddFile(filePath, new MockFileData("backup content")
            {
                CreationTime = creationTime
            });
        }

        var service = new FileManagerService(mockFileSystem);

        // Act - 保留最新的 3 個檔案
        var deletedCount = service.CleanupOldBackups(backupDirectory, pattern, keepCount: 3);

        // Assert
        deletedCount.Should().Be(2);

        // 驗證保留的檔案（最新的 3 個）
        mockFileSystem.File.Exists(@"C:\backups\backup_003.txt").Should().BeTrue();
        mockFileSystem.File.Exists(@"C:\backups\backup_004.txt").Should().BeTrue();
        mockFileSystem.File.Exists(@"C:\backups\backup_005.txt").Should().BeTrue();

        // 驗證刪除的檔案（最舊的 2 個）
        mockFileSystem.File.Exists(@"C:\backups\backup_001.txt").Should().BeFalse();
        mockFileSystem.File.Exists(@"C:\backups\backup_002.txt").Should().BeFalse();
    }

    [Fact]
    public void CleanupOldBackups_當目錄不存在_應回傳0()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string nonexistentDirectory = @"C:\nonexistent";
        const string pattern = "*.bak";

        var service = new FileManagerService(mockFileSystem);

        // Act
        var deletedCount = service.CleanupOldBackups(nonexistentDirectory, pattern, keepCount: 5);

        // Assert
        deletedCount.Should().Be(0);
    }

    [Fact]
    public void GetFileInfo_當檔案存在_應回傳正確的檔案資訊()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\test.txt";
        const string fileContent = "測試檔案內容";
        var creationTime = new DateTime(2024, 1, 15, 10, 30, 0);
        var lastWriteTime = new DateTime(2024, 1, 16, 14, 45, 0);

        mockFileSystem.AddFile(filePath, new MockFileData(fileContent)
        {
            CreationTime = creationTime,
            LastWriteTime = lastWriteTime
        });

        var service = new FileManagerService(mockFileSystem);

        // Act
        var fileInfo = service.GetFileInfo(filePath);

        // Assert
        fileInfo.Should().NotBeNull();
        fileInfo!.Name.Should().Be("test.txt");
        fileInfo.FullPath.Should().Be(filePath);
        fileInfo.Size.Should().Be(Encoding.UTF8.GetByteCount(fileContent));
        fileInfo.CreationTime.Should().Be(creationTime);
        fileInfo.LastWriteTime.Should().Be(lastWriteTime);
        fileInfo.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GetFileInfo_當檔案不存在_應回傳null()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string nonexistentFile = @"C:\data\missing.txt";

        var service = new FileManagerService(mockFileSystem);

        // Act
        var fileInfo = service.GetFileInfo(nonexistentFile);

        // Assert
        fileInfo.Should().BeNull();
    }

    [Fact]
    public void GetFileInfo_當檔案為唯讀_應正確識別唯讀屬性()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\readonly.txt";
        const string fileContent = "唯讀檔案";

        var fileData = new MockFileData(fileContent);
        fileData.Attributes = FileAttributes.ReadOnly;
        mockFileSystem.AddFile(filePath, fileData);

        var service = new FileManagerService(mockFileSystem);

        // Act
        var fileInfo = service.GetFileInfo(filePath);

        // Assert
        fileInfo.Should().NotBeNull();
        fileInfo!.IsReadOnly.Should().BeTrue();
    }

    [Theory]
    [InlineData("document.pdf", @"C:\backup", @"C:\backup\document.pdf")]
    [InlineData("report.xlsx", @"C:\archive\2024", @"C:\archive\2024\report.xlsx")]
    [InlineData("image.jpg", @"C:\temp", @"C:\temp\image.jpg")]
    public void CopyFileToDirectory_使用不同檔案類型_應正確複製(string fileName, string targetDir, string expectedPath)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        var sourceFile = $@"C:\source\{fileName}";
        const string fileContent = "測試內容";

        mockFileSystem.AddFile(sourceFile, new MockFileData(fileContent));

        var service = new FileManagerService(mockFileSystem);

        // Act
        var result = service.CopyFileToDirectory(sourceFile, targetDir);

        // Assert
        result.Should().Be(expectedPath);
        mockFileSystem.File.Exists(expectedPath).Should().BeTrue();
    }

    [Fact]
    public void CopyFileToDirectory_各種檔案名稱_應正確處理()
    {
        // Arrange
        var testCases = new[]
        {
            "simple.txt",
            "file with spaces.txt",
            "file-with-hyphens.txt",
            "file_with_underscores.txt"
        };

        var mockFileSystem = new MockFileSystem();
        var service = new FileManagerService(mockFileSystem);

        foreach (var fileName in testCases)
        {
            // Arrange
            var sourceFile = $@"C:\source\{fileName}";
            mockFileSystem.AddFile(sourceFile, new MockFileData("test content"));

            // Act
            var result = service.CopyFileToDirectory(sourceFile, @"C:\target");

            // Assert
            result.Should().Be($@"C:\target\{fileName}");
            mockFileSystem.File.Exists(result).Should().BeTrue();
        }
    }
}