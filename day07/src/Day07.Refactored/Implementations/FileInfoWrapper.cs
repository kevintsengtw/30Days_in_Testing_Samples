using Day07.Refactored.Abstractions;

namespace Day07.Refactored.Implementations;

/// <summary>
/// class FileInfoWrapper - 檔案資訊包裝器
/// </summary>
public class FileInfoWrapper : IFileInfo
{
    private readonly FileInfo _fileInfo;

    /// <summary>
    /// 檔案資訊包裝器建構子
    /// </summary>
    /// <param name="fileInfo">檔案資訊</param>
    public FileInfoWrapper(FileInfo fileInfo)
    {
        this._fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
    }

    /// <summary>
    /// 獲取檔案大小
    /// </summary>
    public long Length => this._fileInfo.Length;

    /// <summary>
    /// 獲取檔案名稱
    /// </summary>
    public string Name => this._fileInfo.Name;

    /// <summary>
    /// 獲取檔案完整名稱
    /// </summary>
    public string FullName => this._fileInfo.FullName;
}