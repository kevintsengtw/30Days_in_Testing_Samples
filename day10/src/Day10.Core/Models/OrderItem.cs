namespace Day10.Core.Models;

/// <summary>
/// 訂單項目
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 產品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 產品
    /// </summary>
    public Product? Product { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }
}