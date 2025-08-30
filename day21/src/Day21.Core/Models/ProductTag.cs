namespace Day21.Core.Models;

/// <summary>
/// 商品標籤關聯表
/// </summary>
public class ProductTag
{
    /// <summary>
    /// 商品識別碼
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 標籤識別碼
    /// </summary>
    public int TagId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// 標籤
    /// </summary>
    public virtual Tag Tag { get; set; } = null!;
}