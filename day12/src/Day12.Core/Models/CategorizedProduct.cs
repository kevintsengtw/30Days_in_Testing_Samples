namespace Day12.Core.Models;

/// <summary>
/// class CategorizedProduct - 分類產品資料
/// </summary>
public class CategorizedProduct
{
    /// <summary>
    /// 產品資料
    /// </summary>
    public Product Product { get; set; } = new();

    /// <summary>
    /// 分類名稱
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>
    /// 分類代碼
    /// </summary>
    public string CategoryCode { get; set; } = string.Empty;
}