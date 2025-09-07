namespace TUnit.Advanced.Core.Models;

/// <summary>
/// 訂單項目實體類別
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 產品編號
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 項目總價
    /// </summary>
    public decimal TotalPrice => UnitPrice * Quantity;
}