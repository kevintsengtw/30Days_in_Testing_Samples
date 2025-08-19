namespace Day10.Core.Dtos;

/// <summary>
/// 建立客戶請求
/// </summary>
public class CreateCustomerRequest
{
    /// <summary>
    /// 客戶姓名
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 年齡
    /// </summary>
    [Range(18, 120)]
    public int Age { get; set; }

    /// <summary>
    /// 電話號碼
    /// </summary>
    [Phone]
    public string Phone { get; set; } = string.Empty;
}