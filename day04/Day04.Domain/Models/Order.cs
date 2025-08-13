namespace Day04.Domain.Models;

/// <summary>
/// class Order - 用於表示訂單的模型。
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 訂單項目
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 運送地址
    /// </summary>
    public Address? ShippingAddress { get; set; }

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount => Items.Sum(item => item.Price * item.Quantity);
}