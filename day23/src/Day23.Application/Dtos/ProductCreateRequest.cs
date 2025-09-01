namespace Day23.Application.Dtos;

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
    /// 產品價格
    /// </summary>
    public decimal Price { get; set; }
}