namespace Day07.Legacy;

/// <summary>
/// class BackupResult - 備份結果
/// </summary>
public class BackupResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 備份路徑
    /// </summary>
    public string? BackupPath { get; set; }
}