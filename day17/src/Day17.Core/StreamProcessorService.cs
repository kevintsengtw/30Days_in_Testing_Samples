using System.Security.Cryptography;

namespace Day17.Core;

/// <summary>
/// 檔案串流處理服務
/// </summary>
public class StreamProcessorService
{
    private readonly IFileSystem _fileSystem;

    public StreamProcessorService(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
    }

    /// <summary>
    /// 處理文字檔案，對每一行套用轉換函數
    /// </summary>
    /// <param name="inputFilePath">輸入檔案路徑</param>
    /// <param name="outputFilePath">輸出檔案路徑</param>
    /// <param name="transform">轉換函數</param>
    public async Task ProcessTextFileAsync(string inputFilePath, string outputFilePath, Func<string, string> transform)
    {
        if (!this._fileSystem.File.Exists(inputFilePath))
        {
            throw new FileNotFoundException($"輸入檔案不存在: {inputFilePath}");
        }

        var outputDirectory = this._fileSystem.Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(outputDirectory) && !this._fileSystem.Directory.Exists(outputDirectory))
        {
            this._fileSystem.Directory.CreateDirectory(outputDirectory);
        }

        await using var inputStream = this._fileSystem.File.OpenRead(inputFilePath);
        await using var outputStream = this._fileSystem.File.Create(outputFilePath);
        using var reader = new StreamReader(inputStream);
        await using var writer = new StreamWriter(outputStream);

        while (await reader.ReadLineAsync() is { } line)
        {
            var transformedLine = transform(line);
            await writer.WriteLineAsync(transformedLine);
        }
    }

    /// <summary>
    /// 計算檔案的雜湊值
    /// </summary>
    /// <param name="filePath">檔案路徑</param>
    /// <returns>MD5 雜湊值</returns>
    public async Task<string> CalculateFileHashAsync(string filePath)
    {
        if (!this._fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException($"檔案不存在: {filePath}");
        }

        await using var stream = this._fileSystem.File.OpenRead(filePath);
        using var md5 = MD5.Create();

        var hashBytes = await Task.Run(() => md5.ComputeHash(stream));
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// 比較兩個檔案是否相同
    /// </summary>
    /// <param name="filePath1">第一個檔案路徑</param>
    /// <param name="filePath2">第二個檔案路徑</param>
    /// <returns>檔案是否相同</returns>
    public async Task<bool> CompareFilesAsync(string filePath1, string filePath2)
    {
        if (!this._fileSystem.File.Exists(filePath1) || !this._fileSystem.File.Exists(filePath2))
        {
            return false;
        }

        // 先比較檔案大小
        var fileInfo1 = this._fileSystem.FileInfo.New(filePath1);
        var fileInfo2 = this._fileSystem.FileInfo.New(filePath2);

        if (fileInfo1.Length != fileInfo2.Length)
        {
            return false;
        }

        // 比較雜湊值
        var hash1 = await this.CalculateFileHashAsync(filePath1);
        var hash2 = await this.CalculateFileHashAsync(filePath2);

        return hash1.Equals(hash2, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 統計檔案行數和字數
    /// </summary>
    /// <param name="filePath">檔案路徑</param>
    /// <returns>檔案統計資訊</returns>
    public async Task<FileStatistics> GetFileStatisticsAsync(string filePath)
    {
        if (!this._fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException($"檔案不存在: {filePath}");
        }

        await using var stream = this._fileSystem.File.OpenRead(filePath);
        using var reader = new StreamReader(stream);

        var stats = new FileStatistics();

        while (await reader.ReadLineAsync() is { } line)
        {
            stats.LineCount++;
            stats.CharacterCount += line.Length;
            stats.WordCount += line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }

        return stats;
    }

    /// <summary>
    /// 檔案統計資訊
    /// </summary>
    public class FileStatistics
    {
        /// <summary>
        /// 行數
        /// </summary>
        public int LineCount { get; set; }

        /// <summary>
        /// 字數
        /// </summary>
        public int WordCount { get; set; }

        /// <summary>
        /// 字元數
        /// </summary>
        public int CharacterCount { get; set; }
    }
}