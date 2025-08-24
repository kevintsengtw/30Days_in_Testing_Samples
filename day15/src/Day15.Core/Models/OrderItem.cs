namespace Day15.Core.Models;

/// <summary>
/// 訂單項目實體
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 產品
    /// </summary>
    public Product Product { get; set; } = new();

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 總價
    /// </summary>
    public decimal TotalPrice => this.Quantity * this.UnitPrice;
}