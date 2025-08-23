namespace Day14.Core.Models;

/// <summary>
/// 訂單資訊
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 訂單號碼
    /// </summary>
    public string OrderNumber { get; set; } = "";

    /// <summary>
    /// 客戶名稱
    /// </summary>
    public string CustomerName { get; set; } = "";

    /// <summary>
    /// 客戶信箱
    /// </summary>
    public string CustomerEmail { get; set; } = "";

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 訂單項目
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 小計
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// 運費
    /// </summary>
    public decimal ShippingFee { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }
}