namespace Day07.Tests;

/// <summary>
/// class FileBackupServiceTests - 檔案備份服務測試
/// </summary>
public class FileBackupServiceTests
{
    private readonly IFileSystem _fileSystem;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IBackupRepository _backupRepository;
    private readonly ILogger<FileBackupService> _logger;
    private readonly FileBackupService _sut; // System Under Test

    public FileBackupServiceTests()
    {
        this._fileSystem = Substitute.For<IFileSystem>();
        this._dateTimeProvider = Substitute.For<IDateTimeProvider>();
        this._backupRepository = Substitute.For<IBackupRepository>();
        this._logger = Substitute.For<ILogger<FileBackupService>>();

        this._sut = new FileBackupService(this._fileSystem, this._dateTimeProvider, this._backupRepository, this._logger);
    }

    [Fact]
    public async Task BackupFileAsync_來源檔案存在且大小合理_應回傳成功結果()
    {
        // Arrange - 設定 Stub 行為
        const string sourcePath = @"C:\source\test.txt";
        const string destinationPath = @"C:\backup";
        var testTime = new DateTime(2024, 1, 1, 12, 0, 0);

        const string expectedBackupPath = @"C:\backup\test_20240101_120000.txt";

        this._fileSystem.FileExists(sourcePath).Returns(true);

        var fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Length.Returns(1024);
        this._fileSystem.GetFileInfo(sourcePath).Returns(fileInfo);

        this._dateTimeProvider.Now.Returns(testTime);

        // Act
        var actual = await this._sut.BackupFileAsync(sourcePath, destinationPath);

        // Assert
        actual.Success.Should().BeTrue();
        actual.BackupPath.Should().Be(expectedBackupPath);
    }

    [Fact]
    public async Task BackupFileAsync_來源檔案不存在_應記錄警告並回傳失敗()
    {
        // Arrange
        const string sourcePath = @"C:\nonexistent\test.txt";
        const string destinationPath = @"C:\backup";

        this._fileSystem.FileExists(sourcePath).Returns(false);

        // Act
        var actual = await this._sut.BackupFileAsync(sourcePath, destinationPath);

        // Assert - 驗證狀態
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("Source file not found");
        actual.BackupPath.Should().BeNull();

        // Assert - 驗證行為（Mock）
        this._logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Source file not found")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        // 確保沒有進行實際的檔案操作
        this._fileSystem.DidNotReceive().CopyFile(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task BackupFileAsync_來源檔案存在且備份成功_應記錄資訊並回傳成功結果()
    {
        // Arrange
        const string sourcePath = @"C:\source\document.pdf";
        const string destinationPath = @"C:\backup";
        var testTime = new DateTime(2024, 1, 15, 14, 30, 0);

        const string expectedBackupPath = @"C:\backup\document_20240115_143000.pdf";

        this._fileSystem.FileExists(sourcePath).Returns(true);

        var fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Length.Returns(1024 * 1024); // 1MB
        this._fileSystem.GetFileInfo(sourcePath).Returns(fileInfo);

        this._dateTimeProvider.Now.Returns(testTime);

        // Act
        var actual = await _sut.BackupFileAsync(sourcePath, destinationPath);

        // Assert - 狀態驗證
        actual.Success.Should().BeTrue();
        actual.BackupPath.Should().Be(expectedBackupPath);

        // Assert - 行為驗證
        this._fileSystem.Received(1).CopyFile(sourcePath, expectedBackupPath);
        await this._backupRepository.Received(1).SaveBackupHistoryAsync(sourcePath, expectedBackupPath, testTime);

        // Assert - 記錄行為驗證
        this._logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Starting backup")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        this._logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Backup completed successfully")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task BackupFileAsync_來源檔案大小超過限制_應記錄警告並回傳失敗()
    {
        // Arrange
        const string sourcePath = @"C:\source\largefile.zip";
        const string destinationPath = @"C:\backup";
        const int largeFileSize = 200 * 1024 * 1024; // 200MB

        this._fileSystem.FileExists(sourcePath).Returns(true);

        var fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Length.Returns(largeFileSize);
        this._fileSystem.GetFileInfo(sourcePath).Returns(fileInfo);

        // Act
        var actual = await this._sut.BackupFileAsync(sourcePath, destinationPath);

        // Assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("File too large");

        this._logger.Received(1).Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("File too large")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        // 確保沒有執行備份操作
        this._fileSystem.DidNotReceive()
            .CopyFile(Arg.Any<string>(), Arg.Any<string>());

        await this._backupRepository.DidNotReceive()
                  .SaveBackupHistoryAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>());
    }

    [Fact]
    public async Task BackupFileAsync_資料庫儲存歷史記錄時拋出例外_應記錄錯誤並回傳失敗()
    {
        // Arrange
        const string sourcePath = @"C:\source\test.txt";
        const string destinationPath = @"C:\backup";
        var expectedException = new InvalidOperationException("Database connection failed");

        this._fileSystem.FileExists(sourcePath).Returns(true);

        var fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Length.Returns(1024);
        this._fileSystem.GetFileInfo(sourcePath).Returns(fileInfo);

        this._dateTimeProvider.Now.Returns(new DateTime(2024, 1, 1));
        this._backupRepository.When(x => x.SaveBackupHistoryAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<DateTime>()))
            .Do(_ => throw expectedException);

        // Act
        var actual = await this._sut.BackupFileAsync(sourcePath, destinationPath);

        // Assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("Database connection failed");

        this._logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Backup failed")),
            expectedException,
            Arg.Any<Func<object, Exception?, string>>());

        // 確保檔案複製仍然執行了（在例外之前）
        this._fileSystem.Received(1).CopyFile(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task BackupFileAsync_多次呼叫時_應產生唯一備份路徑()
    {
        // Arrange
        const string sourcePath = @"C:\source\test.txt";
        const string destinationPath = @"C:\backup";
        var firstTime = new DateTime(2024, 1, 1, 10, 0, 0);
        var secondTime = new DateTime(2024, 1, 1, 10, 0, 1);

        this._fileSystem.FileExists(sourcePath).Returns(true);

        var fileInfo = Substitute.For<IFileInfo>();
        fileInfo.Length.Returns(1024);
        this._fileSystem.GetFileInfo(sourcePath).Returns(fileInfo);

        this._dateTimeProvider.Now.Returns(firstTime, secondTime);

        // Act
        var firstResult = await this._sut.BackupFileAsync(sourcePath, destinationPath);
        var secondResult = await this._sut.BackupFileAsync(sourcePath, destinationPath);

        // Assert - 驗證時間戳功能
        firstResult.Success.Should().BeTrue();
        secondResult.Success.Should().BeTrue();
        firstResult.BackupPath.Should().NotBe(secondResult.BackupPath);
        firstResult.BackupPath.Should().Be(@"C:\backup\test_20240101_100000.txt");
        secondResult.BackupPath.Should().Be(@"C:\backup\test_20240101_100001.txt");
    }
}