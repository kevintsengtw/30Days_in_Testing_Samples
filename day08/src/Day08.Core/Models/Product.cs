namespace Day08.Core.Models;

/// <summary>
/// class Product - 商品資訊
/// </summary>
public class Product
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 商品分類
    /// </summary>
    public string Category { get; set; } = string.Empty;
}