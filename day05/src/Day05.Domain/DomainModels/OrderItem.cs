namespace Day05.Domain.DomainModels;

/// <summary>
/// class OrderItem - 訂單項目
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 訂單項目識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 產品識別碼
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 訂單項目數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 訂單項目單價
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 訂單項目新增時間
    /// </summary>
    public DateTime AddedAt { get; set; }

    /// <summary>
    /// 訂單項目修改時間
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}