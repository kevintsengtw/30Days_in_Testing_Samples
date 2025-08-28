namespace Day19.WebApplication.Entities;

/// <summary>
/// 貨運商實體
/// </summary>
public class Shipper
{
    /// <summary>
    /// 貨運商識別碼
    /// </summary>
    [Key]
    public int ShipperId { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡電話
    /// </summary>
    [StringLength(20)]
    public string? Phone { get; set; }
}