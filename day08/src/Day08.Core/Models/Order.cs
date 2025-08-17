namespace Day08.Core.Models;

/// <summary>
/// class Order - 訂單資訊
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 客戶編號
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// 商品編號
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// 購買數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 訂單項目清單
    /// </summary>
    public OrderItem[] Items { get; set; } = Array.Empty<OrderItem>();
}