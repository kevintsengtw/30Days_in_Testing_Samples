namespace Day07.Refactored.Abstractions;

/// <summary>
/// interface IFileSystem - 檔案系統介面
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// 檢查檔案是否存在
    /// </summary>
    /// <param name="path">檔案路徑</param>
    /// <returns>是否存在</returns>
    bool FileExists(string path);

    /// <summary>
    /// 獲取檔案資訊
    /// </summary>
    /// <param name="path">檔案路徑</param>
    /// <returns>檔案資訊</returns>
    IFileInfo GetFileInfo(string path);

    /// <summary>
    /// 複製檔案
    /// </summary>
    /// <param name="sourcePath">來源路徑</param>
    /// <param name="destinationPath">目標路徑</param>
    void CopyFile(string sourcePath, string destinationPath);
}