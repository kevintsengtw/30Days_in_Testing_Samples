namespace Day12.Core.Models;

/// <summary>
/// class ProductCsvRecord - CSV 產品資料記錄
/// </summary>
public class ProductCsvRecord
{
    /// <summary>
    /// 產品識別碼
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 產品分類
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 產品價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 是否可販售
    /// </summary>
    public bool IsAvailable { get; set; }
}