namespace Day15.Core.Models;

/// <summary>
/// 訂單實體
/// </summary>
public class Order
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 客戶
    /// </summary>
    public User Customer { get; set; } = new();

    /// <summary>
    /// 訂單項目列表
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; }
}