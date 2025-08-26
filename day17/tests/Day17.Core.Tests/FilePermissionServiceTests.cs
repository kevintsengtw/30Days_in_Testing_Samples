namespace Day17.Core.Tests;

/// <summary>
/// FilePermissionService 的單元測試
/// </summary>
public class FilePermissionServiceTests
{
    [Fact]
    public void CanReadFile_當檔案存在且可讀取_應回傳true()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\readable.txt";
        const string fileContent = "可讀取的檔案內容";

        mockFileSystem.AddFile(filePath, new MockFileData(fileContent));

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanReadFile(filePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanReadFile_當檔案不存在_應回傳false()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string nonexistentFile = @"C:\data\missing.txt";

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanReadFile(nonexistentFile);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanReadFile_當檔案無法讀取_應回傳false()
    {
        // Arrange
        var mockFileSystem = Substitute.For<IFileSystem>();
        var mockFile = Substitute.For<IFile>();
        mockFileSystem.File.Returns(mockFile);

        const string filePath = @"C:\data\protected.txt";

        mockFile.Exists(filePath).Returns(true);
        mockFile.OpenRead(filePath).Returns(x => { throw new UnauthorizedAccessException("存取被拒"); });

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanReadFile(filePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanWriteFile_當檔案存在且可寫入_應回傳true()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\writable.txt";
        const string fileContent = "可寫入的檔案";

        mockFileSystem.AddFile(filePath, new MockFileData(fileContent));

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteFile(filePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanWriteFile_當檔案為唯讀_應回傳false()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\readonly.txt";
        const string fileContent = "唯讀檔案";

        var fileData = new MockFileData(fileContent);
        fileData.Attributes = FileAttributes.ReadOnly;
        mockFileSystem.AddFile(filePath, fileData);

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteFile(filePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanWriteFile_當檔案不存在但可在目錄中建立_應回傳true()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\newfile.txt";

        // 建立目錄但不建立檔案
        mockFileSystem.AddDirectory(@"C:\data");

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteFile(filePath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanWriteFile_當檔案不存在且目錄不存在_應回傳false()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\nonexistent\newfile.txt";

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteFile(filePath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanWriteToDirectory_當目錄存在且可寫入_應回傳true()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string directoryPath = @"C:\temp";

        mockFileSystem.AddDirectory(directoryPath);

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteToDirectory(directoryPath);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanWriteToDirectory_當目錄不存在_應回傳false()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string nonexistentDirectory = @"C:\nonexistent";

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteToDirectory(nonexistentDirectory);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanWriteToDirectory_當目錄存在但無寫入權限_應回傳false()
    {
        // Arrange
        var mockFileSystem = Substitute.For<IFileSystem>();
        var mockDirectory = Substitute.For<IDirectory>();
        var mockFile = Substitute.For<IFile>();
        var mockPath = Substitute.For<IPath>();

        mockFileSystem.Directory.Returns(mockDirectory);
        mockFileSystem.File.Returns(mockFile);
        mockFileSystem.Path.Returns(mockPath);

        const string directoryPath = @"C:\protected";

        mockDirectory.Exists(directoryPath).Returns(true);
        mockPath.Combine(directoryPath, Arg.Any<string>()).Returns(@"C:\protected\temp_test.tmp");
        mockFile.When(x => x.WriteAllText(Arg.Any<string>(), Arg.Any<string>()))
                .Do(_ => throw new UnauthorizedAccessException("存取被拒"));

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteToDirectory(directoryPath);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetFilePermissions_當檔案存在_應回傳完整權限摘要()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\test.txt";
        const string fileContent = "測試檔案";

        mockFileSystem.AddFile(filePath, new MockFileData(fileContent));

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var permissions = service.GetFilePermissions(filePath);

        // Assert
        permissions.Should().NotBeNull();
        permissions.FilePath.Should().Be(filePath);
        permissions.Exists.Should().BeTrue();
        permissions.CanRead.Should().BeTrue();
        permissions.CanWrite.Should().BeTrue();
        permissions.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GetFilePermissions_當檔案不存在_應回傳適當的權限摘要()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string nonexistentFile = @"C:\data\missing.txt";

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var permissions = service.GetFilePermissions(nonexistentFile);

        // Assert
        permissions.Should().NotBeNull();
        permissions.FilePath.Should().Be(nonexistentFile);
        permissions.Exists.Should().BeFalse();
        permissions.CanRead.Should().BeFalse();
        permissions.CanWrite.Should().BeFalse();
        permissions.IsReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GetFilePermissions_當檔案為唯讀_應正確反映唯讀狀態()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\readonly.txt";
        const string fileContent = "唯讀檔案";

        var fileData = new MockFileData(fileContent);
        fileData.Attributes = FileAttributes.ReadOnly;
        mockFileSystem.AddFile(filePath, fileData);

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var permissions = service.GetFilePermissions(filePath);

        // Assert
        permissions.Should().NotBeNull();
        permissions.FilePath.Should().Be(filePath);
        permissions.Exists.Should().BeTrue();
        permissions.CanRead.Should().BeTrue();
        permissions.CanWrite.Should().BeFalse();
        permissions.IsReadOnly.Should().BeTrue();
    }

    [Theory]
    [InlineData(@"C:\public\file.txt", true)]
    [InlineData(@"C:\private\file.txt", false)]
    [InlineData(@"C:\temp\file.txt", true)]
    public void CanReadFile_測試不同路徑的檔案讀取權限(string filePath, bool expectedCanRead)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();

        if (expectedCanRead)
        {
            mockFileSystem.AddFile(filePath, new MockFileData("測試內容"));
        }
        else
        {
            // 模擬無法讀取的檔案
            var mockFile = Substitute.For<IFile>();
            var realMockFileSystem = Substitute.For<IFileSystem>();
            realMockFileSystem.File.Returns(mockFile);

            mockFile.Exists(filePath).Returns(true);
            mockFile.OpenRead(filePath).Returns(x => { throw new UnauthorizedAccessException(); });

            var service = new FilePermissionService(realMockFileSystem);

            // Act
            var result = service.CanReadFile(filePath);

            // Assert
            result.Should().Be(expectedCanRead);
            return;
        }

        var normalService = new FilePermissionService(mockFileSystem);

        // Act
        var normalResult = normalService.CanReadFile(filePath);

        // Assert
        normalResult.Should().Be(expectedCanRead);
    }

    [Fact]
    public void CanWriteFile_當發生一般例外_應回傳false()
    {
        // Arrange
        var mockFileSystem = Substitute.For<IFileSystem>();
        var mockFile = Substitute.For<IFile>();
        var mockFileInfo = Substitute.For<IFileInfo>();
        var mockFileInfoFactory = Substitute.For<IFileInfoFactory>();

        mockFileSystem.File.Returns(mockFile);
        mockFileSystem.FileInfo.Returns(mockFileInfoFactory);

        const string filePath = @"C:\data\error.txt";

        mockFile.Exists(filePath).Returns(true);
        mockFileInfoFactory.New(filePath).Returns(mockFileInfo);
        mockFileInfo.IsReadOnly.Returns(false);
        mockFile.OpenWrite(filePath).Returns(_ => throw new IOException("磁碟空間不足"));

        var service = new FilePermissionService(mockFileSystem);

        // Act
        var result = service.CanWriteFile(filePath);

        // Assert
        result.Should().BeFalse();
    }
}