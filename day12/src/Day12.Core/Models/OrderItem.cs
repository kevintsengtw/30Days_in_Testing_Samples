namespace Day12.Core.Models;

/// <summary>
/// class OrderItem - 訂單項目資料
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 產品識別碼
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 產品資料
    /// </summary>
    public Product Product { get; set; } = new();

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }
}