namespace Day05.Domain.DomainModels;

/// <summary>
/// class PaymentResult - 付款結果
/// </summary>
public class PaymentResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// 交易識別碼
    /// </summary>
    public string TransactionId { get; set; } = string.Empty;
}