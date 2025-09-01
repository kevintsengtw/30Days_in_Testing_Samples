namespace Day23.Application.Dtos;

/// <summary>
/// 更新產品請求
/// </summary>
public class ProductUpdateRequest
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