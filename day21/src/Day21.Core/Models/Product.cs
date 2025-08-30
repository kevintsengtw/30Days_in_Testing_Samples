namespace Day21.Core.Models;

/// <summary>
/// 商品
/// </summary>
public class Product
{
    /// <summary>
    /// 商品識別碼
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 折扣價格
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPrice { get; set; }

    /// <summary>
    /// 庫存數量
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    [MaxLength(50)]
    public string? SKU { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 分類識別碼
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 商品分類
    /// </summary>
    public virtual Category Category { get; set; } = null!;

    /// <summary>
    /// 商品標籤
    /// </summary>
    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

    /// <summary>
    /// 訂單項目
    /// </summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}