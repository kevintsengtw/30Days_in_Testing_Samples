namespace Day17.Core.Tests;

/// <summary>
/// StreamProcessorService 的單元測試
/// </summary>
public class StreamProcessorServiceTests
{
    [Fact]
    public async Task ProcessTextFileAsync_當輸入檔案存在_應正確轉換並輸出()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string inputPath = @"C:\input\data.txt";
        const string outputPath = @"C:\output\processed.txt";
        const string inputContent = "line1\nline2\nline3";

        mockFileSystem.AddFile(inputPath, new MockFileData(inputContent));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        await service.ProcessTextFileAsync(inputPath, outputPath, line => line.ToUpper());

        // Assert
        mockFileSystem.File.Exists(outputPath).Should().BeTrue();
        var outputContent = await mockFileSystem.File.ReadAllTextAsync(outputPath);
        var lines = outputContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        lines.Should().HaveCount(3);
        lines[0].Should().Be("LINE1");
        lines[1].Should().Be("LINE2");
        lines[2].Should().Be("LINE3");
    }

    [Fact]
    public async Task ProcessTextFileAsync_當輸入檔案不存在_應拋出FileNotFoundException()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string inputPath = @"C:\input\missing.txt";
        const string outputPath = @"C:\output\processed.txt";

        var service = new StreamProcessorService(mockFileSystem);

        // Act & Assert
        var action = async () => await service.ProcessTextFileAsync(inputPath, outputPath, line => line);
        await action.Should().ThrowAsync<FileNotFoundException>()
                    .WithMessage($"輸入檔案不存在: {inputPath}");
    }

    [Fact]
    public async Task ProcessTextFileAsync_當輸出目錄不存在_應建立目錄()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string inputPath = @"C:\input\data.txt";
        const string outputPath = @"C:\new\output\dir\processed.txt";
        const string inputContent = "test line";

        mockFileSystem.AddFile(inputPath, new MockFileData(inputContent));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        await service.ProcessTextFileAsync(inputPath, outputPath, line => line.ToLower());

        // Assert
        mockFileSystem.Directory.Exists(@"C:\new\output\dir").Should().BeTrue();
        mockFileSystem.File.Exists(outputPath).Should().BeTrue();
    }

    [Theory]
    [InlineData("Hello World", "HELLO WORLD")]
    [InlineData("Mixed Case Text", "MIXED CASE TEXT")]
    [InlineData("123 Numbers", "123 NUMBERS")]
    public async Task ProcessTextFileAsync_使用不同轉換函數_應正確處理(string input, string expected)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string inputPath = @"C:\test\input.txt";
        const string outputPath = @"C:\test\output.txt";

        mockFileSystem.AddFile(inputPath, new MockFileData(input));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        await service.ProcessTextFileAsync(inputPath, outputPath, line => line.ToUpper());

        // Assert
        var result = (await mockFileSystem.File.ReadAllTextAsync(outputPath)).Trim();
        result.Should().Be(expected);
    }

    [Fact]
    public async Task CalculateFileHashAsync_當檔案存在_應回傳正確的MD5雜湊值()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\test.txt";
        const string fileContent = "Hello, World!";

        mockFileSystem.AddFile(filePath, new MockFileData(fileContent));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var hash = await service.CalculateFileHashAsync(filePath);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().HaveLength(32);               // MD5 雜湊值長度
        hash.Should().MatchRegex("^[A-F0-9]{32}$"); // 十六進位大寫格式
    }

    [Fact]
    public async Task CalculateFileHashAsync_相同內容的檔案_應產生相同雜湊值()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string file1Path = @"C:\data\file1.txt";
        const string file2Path = @"C:\data\file2.txt";
        const string sameContent = "相同的檔案內容";

        mockFileSystem.AddFile(file1Path, new MockFileData(sameContent));
        mockFileSystem.AddFile(file2Path, new MockFileData(sameContent));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var hash1 = await service.CalculateFileHashAsync(file1Path);
        var hash2 = await service.CalculateFileHashAsync(file2Path);

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public async Task CalculateFileHashAsync_不同內容的檔案_應產生不同雜湊值()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string file1Path = @"C:\data\file1.txt";
        const string file2Path = @"C:\data\file2.txt";

        mockFileSystem.AddFile(file1Path, new MockFileData("內容1"));
        mockFileSystem.AddFile(file2Path, new MockFileData("內容2"));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var hash1 = await service.CalculateFileHashAsync(file1Path);
        var hash2 = await service.CalculateFileHashAsync(file2Path);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public async Task CalculateFileHashAsync_當檔案不存在_應拋出FileNotFoundException()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string missingFile = @"C:\data\missing.txt";

        var service = new StreamProcessorService(mockFileSystem);

        // Act & Assert
        var action = async () => await service.CalculateFileHashAsync(missingFile);
        await action.Should().ThrowAsync<FileNotFoundException>()
                    .WithMessage($"檔案不存在: {missingFile}");
    }

    [Fact]
    public async Task CompareFilesAsync_當兩個檔案內容相同_應回傳true()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string file1Path = @"C:\data\file1.txt";
        const string file2Path = @"C:\data\file2.txt";
        const string sameContent = "相同的檔案內容用於比較測試";

        mockFileSystem.AddFile(file1Path, new MockFileData(sameContent));
        mockFileSystem.AddFile(file2Path, new MockFileData(sameContent));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var result = await service.CompareFilesAsync(file1Path, file2Path);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CompareFilesAsync_當兩個檔案內容不同_應回傳false()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string file1Path = @"C:\data\file1.txt";
        const string file2Path = @"C:\data\file2.txt";

        mockFileSystem.AddFile(file1Path, new MockFileData("檔案1的內容"));
        mockFileSystem.AddFile(file2Path, new MockFileData("檔案2的內容"));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var result = await service.CompareFilesAsync(file1Path, file2Path);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CompareFilesAsync_當檔案大小不同_應快速回傳false()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string file1Path = @"C:\data\small.txt";
        const string file2Path = @"C:\data\large.txt";

        mockFileSystem.AddFile(file1Path, new MockFileData("小"));
        mockFileSystem.AddFile(file2Path, new MockFileData("這是一個比較大的檔案內容"));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var result = await service.CompareFilesAsync(file1Path, file2Path);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(@"C:\missing1.txt", @"C:\exists.txt")]
    [InlineData(@"C:\exists.txt", @"C:\missing2.txt")]
    [InlineData(@"C:\missing1.txt", @"C:\missing2.txt")]
    public async Task CompareFilesAsync_當任一檔案不存在_應回傳false(string file1, string file2)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(@"C:\exists.txt", new MockFileData("存在的檔案"));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var result = await service.CompareFilesAsync(file1, file2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetFileStatisticsAsync_當檔案存在_應回傳正確統計資訊()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\stats.txt";
        const string fileContent = "第一行 有 三個 字\n第二行 有 四個 中文 字\n第三行有五個英文word here";

        mockFileSystem.AddFile(filePath, new MockFileData(fileContent));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var stats = await service.GetFileStatisticsAsync(filePath);

        // Assert
        stats.Should().NotBeNull();
        stats.LineCount.Should().Be(3);
        stats.CharacterCount.Should().BeGreaterThan(0);
        stats.WordCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetFileStatisticsAsync_當檔案為空_應回傳零統計()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\data\empty.txt";

        mockFileSystem.AddFile(filePath, new MockFileData(""));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var stats = await service.GetFileStatisticsAsync(filePath);

        // Assert
        stats.Should().NotBeNull();
        stats.LineCount.Should().Be(0);
        stats.CharacterCount.Should().Be(0);
        stats.WordCount.Should().Be(0);
    }

    [Fact]
    public async Task GetFileStatisticsAsync_當檔案不存在_應拋出FileNotFoundException()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string missingFile = @"C:\data\missing.txt";

        var service = new StreamProcessorService(mockFileSystem);

        // Act & Assert
        var action = async () => await service.GetFileStatisticsAsync(missingFile);
        await action.Should().ThrowAsync<FileNotFoundException>()
                    .WithMessage($"檔案不存在: {missingFile}");
    }

    [Theory]
    [InlineData("Hello World", 1, 2, 11)]
    [InlineData("Line 1\nLine 2", 2, 4, 12)]
    [InlineData("One\nTwo\nThree", 3, 3, 11)]
    [InlineData("Multiple   spaces   between", 1, 3, 27)]
    public async Task GetFileStatisticsAsync_驗證不同文字內容的統計結果(string content, int expectedLines, int expectedWords, int expectedChars)
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string filePath = @"C:\test\content.txt";

        mockFileSystem.AddFile(filePath, new MockFileData(content));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        var stats = await service.GetFileStatisticsAsync(filePath);

        // Assert
        stats.LineCount.Should().Be(expectedLines);
        stats.WordCount.Should().Be(expectedWords);
        stats.CharacterCount.Should().Be(expectedChars);
    }

    [Fact]
    public async Task ProcessTextFileAsync_處理空檔案_應建立空的輸出檔案()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();
        const string inputPath = @"C:\input\empty.txt";
        const string outputPath = @"C:\output\empty_processed.txt";

        mockFileSystem.AddFile(inputPath, new MockFileData(""));

        var service = new StreamProcessorService(mockFileSystem);

        // Act
        await service.ProcessTextFileAsync(inputPath, outputPath, line => line.ToUpper());

        // Assert
        mockFileSystem.File.Exists(outputPath).Should().BeTrue();
        var outputContent = await mockFileSystem.File.ReadAllTextAsync(outputPath);
        outputContent.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessLargeFile_使用串流_記憶體效率測試()
    {
        // Arrange
        var mockFileSystem = new MockFileSystem();

        // 建立一個適中大小的測試檔案，而不是真正的大檔案
        var testContent = string.Join("\n", Enumerable.Range(1, 1000).Select(i => $"Line {i}"));
        mockFileSystem.AddFile("test.txt", new MockFileData(testContent));

        var processor = new StreamProcessorService(mockFileSystem);

        // Act
        var result = await processor.GetFileStatisticsAsync("test.txt");

        // Assert
        result.LineCount.Should().Be(1000);
        result.WordCount.Should().Be(2000); // "Line 1", "Line 2", etc.
    }
}