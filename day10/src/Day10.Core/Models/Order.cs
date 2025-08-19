using Day10.Core.Enums;

namespace Day10.Core.Models;

/// <summary>
/// 訂單
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 訂單號碼
    /// </summary>
    public string OrderNumber { get; } = Guid.NewGuid().ToString("N")[..8];

    /// <summary>
    /// 客戶
    /// </summary>
    public Customer? Customer { get; set; }

    /// <summary>
    /// 訂單項目
    /// </summary>
    public List<OrderItem> Items { get; set; } = [];

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    public DateTime CreatedDate { get; private set; } = DateTime.Now;

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 標籤
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// 中繼資料
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// 分類編號集合
    /// </summary>
    public HashSet<int> CategoryIds { get; set; } = [];

    /// <summary>
    /// 計算總金額
    /// </summary>
    public decimal CalculateTotal()
    {
        return this.Items.Sum(item => item.UnitPrice * item.Quantity);
    }
}