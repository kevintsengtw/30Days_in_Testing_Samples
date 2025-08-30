namespace Day21.Core.Models;

/// <summary>
/// 訂單
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// 客戶識別碼
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// 客戶電子郵件
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 訂單狀態
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 訂單總金額
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

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
    /// 訂單項目
    /// </summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    /// <summary>
    /// 客戶
    /// </summary>
    [ForeignKey("CustomerId")]
    public virtual Customer? Customer { get; set; }
}