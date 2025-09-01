namespace Day23.Domain;

/// <summary>
/// class Product - 產品
/// </summary>
public class Product
{
    /// <summary>
    /// 產品唯一識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 產品價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}