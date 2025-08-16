namespace Day07.Refactored.Abstractions;

/// <summary>
/// interface IFileInfo - 檔案資訊介面
/// </summary>
public interface IFileInfo
{
    /// <summary>
    /// 獲取檔案大小
    /// </summary>
    long Length { get; }

    /// <summary>
    /// 獲取檔案名稱
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 獲取檔案完整名稱
    /// </summary>
    string FullName { get; }
}