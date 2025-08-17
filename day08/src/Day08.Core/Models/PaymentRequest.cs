namespace Day08.Core.Models;

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
    /// 貨幣類型
    /// </summary>
    public string Currency { get; set; } = "TWD";

    /// <summary>
    /// 信用卡號碼（遮罩）
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;
}