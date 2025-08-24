namespace Day15.Core.Models;

/// <summary>
/// 產品實體
/// </summary>
public class Product
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 產品描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 類別
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }
}