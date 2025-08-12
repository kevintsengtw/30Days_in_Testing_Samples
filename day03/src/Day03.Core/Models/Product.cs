namespace Day03.Core.Models;

/// <summary>
/// class Product - 代表產品實體。
/// </summary>
public class Product
{
    /// <summary>
    /// 產品的唯一識別碼。
    /// </summary>
    /// <value></value>
    public int Id { get; set; }

    /// <summary>
    /// 產品名稱。
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 產品價格。
    /// </summary>
    /// <value></value>
    public decimal Price { get; set; }

    /// <summary>
    /// 產品類別。
    /// </summary>
    /// <value></value>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 產品建立時間。
    /// </summary>
    /// <value></value>
    public DateTime CreatedAt { get; set; }
}