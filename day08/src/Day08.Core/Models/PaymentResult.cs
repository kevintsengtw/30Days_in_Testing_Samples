namespace Day08.Core.Models;

/// <summary>
/// class PaymentResult - 付款結果
/// </summary>
public class PaymentResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 交易編號
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;
}