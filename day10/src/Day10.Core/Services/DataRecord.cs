namespace Day10.Core.Services;

/// <summary>
/// 資料記錄
/// </summary>
public class DataRecord
{
    /// <summary>
    /// 記錄編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 資料內容
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// 時間戳記
    /// </summary>
    public DateTime Timestamp { get; set; }
}