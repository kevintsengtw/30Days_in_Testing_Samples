namespace Day10.Core.Services;

/// <summary>
/// 批次處理結果
/// </summary>
public class BatchProcessResult
{
    /// <summary>
    /// 成功數量
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失敗數量
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 處理結果清單
    /// </summary>
    public List<ProcessResult> Results { get; set; } = new();
}