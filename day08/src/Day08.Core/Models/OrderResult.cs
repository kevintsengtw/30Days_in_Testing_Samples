namespace Day08.Core.Models;

/// <summary>
/// class OrderResult - 訂單處理結果
/// </summary>
public class OrderResult
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
    /// 訂單編號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount { get; set; }
}