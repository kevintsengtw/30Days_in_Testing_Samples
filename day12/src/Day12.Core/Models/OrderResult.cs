namespace Day12.Core.Models;

/// <summary>
/// 訂單處理結果模型
/// </summary>
public class OrderResult
{
    /// <summary>
    /// 處理是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 產品資料
    /// </summary>
    public Product Product { get; set; } = new();

    /// <summary>
    /// 客戶資料
    /// </summary>
    public Customer Customer { get; set; } = new();

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 折扣金額
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 最終金額
    /// </summary>
    public decimal FinalAmount { get; set; }

    /// <summary>
    /// 是否需要審核
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// 預估送貨天數
    /// </summary>
    public int EstimatedDeliveryDays { get; set; }
}