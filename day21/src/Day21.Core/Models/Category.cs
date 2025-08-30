namespace Day21.Core.Models;

/// <summary>
/// 商品分類
/// </summary>
public class Category
{
    /// <summary>
    /// 分類識別碼
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 分類名稱
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 分類描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 該分類下的商品
    /// </summary>
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}