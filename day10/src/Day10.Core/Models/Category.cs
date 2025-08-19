namespace Day10.Core.Models;

/// <summary>
/// 分類 (具有循環參考結構)
/// </summary>
public class Category
{
    /// <summary>
    /// 分類編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 分類名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父分類
    /// </summary>
    public Category? Parent { get; set; }

    /// <summary>
    /// 子分類清單
    /// </summary>
    public List<Category> Children { get; set; } = new();
}