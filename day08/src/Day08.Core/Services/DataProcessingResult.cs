namespace Day08.Core.Services;

/// <summary>
/// 資料處理結果
/// </summary>
public class DataProcessingResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 處理筆數
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}