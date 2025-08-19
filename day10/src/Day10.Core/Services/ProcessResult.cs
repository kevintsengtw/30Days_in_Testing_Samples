namespace Day10.Core.Services;

/// <summary>
/// 處理結果
/// </summary>
public class ProcessResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 訂單編號
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }
}