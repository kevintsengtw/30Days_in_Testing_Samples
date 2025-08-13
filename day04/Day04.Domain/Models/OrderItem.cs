namespace Day04.Domain.Models;

/// <summary>
/// class OrderItem - 用於表示訂單項目的模型。
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 訂單項目ID
    /// </summary>
    /// <value></value>
    public int ProductId { get; set; }

    /// <summary>
    /// 訂單項目名稱
    /// </summary>
    /// <value></value>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 訂單項目數量
    /// </summary>
    /// <value></value>
    public int Quantity { get; set; }

    /// <summary>
    /// 訂單項目價格
    /// </summary>
    /// <value></value>
    public decimal Price { get; set; }
}