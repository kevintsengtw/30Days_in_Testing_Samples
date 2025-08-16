using Day07.Refactored.Abstractions;

namespace Day07.Refactored.Implementations;

/// <summary>
/// class FileSystemWrapper - 檔案系統包裝器
/// </summary>
public class FileSystemWrapper : IFileSystem
{
    /// <summary>
    /// 檢查檔案是否存在
    /// </summary>
    /// <param name="path">檔案路徑</param>
    /// <returns>是否存在</returns>
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// 獲取檔案資訊
    /// </summary>
    /// <param name="path">檔案路徑</param>
    /// <returns>檔案資訊</returns>
    public IFileInfo GetFileInfo(string path)
    {
        var fileInfo = new FileInfo(path);
        return new FileInfoWrapper(fileInfo);
    }

    /// <summary>
    /// 複製檔案
    /// </summary>
    /// <param name="sourcePath">來源路徑</param>
    /// <param name="destinationPath">目標路徑</param>
    public void CopyFile(string sourcePath, string destinationPath)
    {
        File.Copy(sourcePath, destinationPath);
    }
}