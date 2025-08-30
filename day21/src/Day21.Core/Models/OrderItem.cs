namespace Day21.Core.Models;

/// <summary>
/// 訂單項目
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 訂單項目識別碼
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 商品識別碼
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 訂單
    /// </summary>
    public virtual Order Order { get; set; } = null!;

    /// <summary>
    /// 商品
    /// </summary>
    public virtual Product Product { get; set; } = null!;
}