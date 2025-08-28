namespace Day19.WebApplication.Models;

/// <summary>
/// 出貨記錄實體
/// </summary>
public class Shipment
{
    public int Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public int RecipientId { get; set; }
    public ShipmentStatus Status { get; set; }
    public decimal Weight { get; set; }
    public decimal Cost { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // 導航屬性
    public virtual Recipient Recipient { get; set; } = null!;
}

/// <summary>
/// 收件者實體
/// </summary>
public class Recipient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 出貨狀態列舉
/// </summary>
public enum ShipmentStatus
{
    Pending = 0,    // 待處理
    Processing = 1, // 處理中
    Shipped = 2,    // 已出貨
    Delivered = 3,  // 已送達
    Cancelled = 4   // 已取消
}