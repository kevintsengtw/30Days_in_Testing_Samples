namespace TUnit.Advanced.Core.Models;

/// <summary>
/// 訂單實體類別
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// 客戶識別碼
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 客戶等級
    /// </summary>
    public CustomerLevel CustomerLevel { get; set; }

    /// <summary>
    /// 訂單項目清單
    /// </summary>
    public List<OrderItem> Items { get; set; } = [];

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.待處理;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 折扣碼
    /// </summary>
    public string? DiscountCode { get; set; }

    /// <summary>
    /// 折扣金額
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// 運費
    /// </summary>
    public decimal ShippingFee { get; set; }

    /// <summary>
    /// 計算總金額（包含折扣和運費）
    /// </summary>
    public decimal TotalAmount => Items.Sum(x => x.TotalPrice) - DiscountAmount + ShippingFee;

    /// <summary>
    /// 計算商品總金額（不含折扣和運費）
    /// </summary>
    public decimal SubTotal => Items.Sum(x => x.TotalPrice);
}