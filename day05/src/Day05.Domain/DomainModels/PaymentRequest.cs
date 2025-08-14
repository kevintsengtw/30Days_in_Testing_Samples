namespace Day05.Domain.DomainModels;

/// <summary>
/// class PaymentRequest - 付款請求
/// </summary>
public class PaymentRequest
{
    /// <summary>
    /// 付款金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 付款幣別
    /// </summary>
    public string Currency { get; set; } = "TWD";

    /// <summary>
    /// 付款方式
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;
}