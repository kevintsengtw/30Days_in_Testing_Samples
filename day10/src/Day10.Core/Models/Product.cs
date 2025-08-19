namespace Day10.Core.Models;

/// <summary>
/// 產品
/// </summary>
public class Product
{
    /// <summary>
    /// 產品編號
    /// </summary>
    public int Id { get; set; }

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

    /// <summary>
    /// 是否有庫存
    /// </summary>
    public bool InStock { get; set; }

    /// <summary>
    /// 評論
    /// </summary>
    public List<Review> Reviews { get; set; } = new();
}

/// <summary>
/// 評論
/// </summary>
public class Review
{
    /// <summary>
    /// 評論編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 評論內容
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 評分
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// 建立日期
    /// </summary>
    public DateTime CreatedDate { get; set; }
}