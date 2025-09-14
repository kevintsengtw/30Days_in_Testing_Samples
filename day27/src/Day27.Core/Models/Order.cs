namespace Day27.Core.Models;

/// <summary>
/// 訂單實體
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 客戶識別碼
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// 產品識別碼
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount => UnitPrice * Quantity;

    /// <summary>
    /// 付款方式
    /// </summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}