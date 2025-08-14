namespace Day05.Domain.DomainModels;

/// <summary>
/// class Product - 產品
/// </summary>
public class Product
{
    /// <summary>
    /// 產品識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 產品價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 產品建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 產品更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}