namespace Day10.Core.Dtos;

/// <summary>
/// 建立產品請求
/// </summary>
public class ProductCreateRequest
{
    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 分類
    /// </summary>
    public string Category { get; set; } = string.Empty;
}