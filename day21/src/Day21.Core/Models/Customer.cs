namespace Day21.Core.Models;

/// <summary>
/// 客戶
/// </summary>
public class Customer
{
    /// <summary>
    /// 客戶識別碼
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// 客戶名稱
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 客戶電子郵件
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 訂單
    /// </summary>
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}