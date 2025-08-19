namespace Day10.Core.Services;

/// <summary>
/// 處理結果
/// </summary>
public class ProcessingResult
{
    /// <summary>
    /// 處理數量
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// 錯誤數量
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// 錯誤清單
    /// </summary>
    public List<string> Errors { get; set; } = new();
}