namespace Day12.Core.Models;

/// <summary>
/// class Product - 產品資料
/// </summary>
public class Product
{
    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 產品價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 是否可販售
    /// </summary>
    public bool IsAvailable { get; set; }

    /// <summary>
    /// 產品描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}