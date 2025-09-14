namespace Day27.Core.Models;

/// <summary>
/// 付款結果
/// </summary>
public class PaymentResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 交易識別碼
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}