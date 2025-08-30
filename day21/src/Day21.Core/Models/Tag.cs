namespace Day21.Core.Models;

/// <summary>
/// 標籤
/// </summary>
public class Tag
{
    /// <summary>
    /// 標籤識別碼
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 標籤名稱
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 與此標籤關聯的商品標籤
    /// </summary>
    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}