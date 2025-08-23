namespace Day14.Core.Models;

/// <summary>
/// 訂單項目
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 項目編號
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string ProductName { get; set; } = "";

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