namespace Day05.Domain.DomainModels;

/// <summary>
/// class Order - 訂單
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 訂單建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 訂單更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 訂單處理時間
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// 訂單項目
    /// </summary>
    public OrderItem[] Items { get; set; } = [];

    /// <summary>
    /// 審計資訊
    /// </summary>
    public AuditInfo? AuditInfo { get; set; }
}