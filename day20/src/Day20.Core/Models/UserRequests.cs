namespace Day20.Core.Models;

/// <summary>
/// 建立使用者請求
/// </summary>
public class UserCreateRequest
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 全名
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 年齡
    /// </summary>
    [Range(1, 150)]
    public int Age { get; set; }
}

/// <summary>
/// 更新使用者請求
/// </summary>
public class UserUpdateRequest
{
    /// <summary>
    /// 電子郵件
    /// </summary>
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    /// <summary>
    /// 全名
    /// </summary>
    [StringLength(100)]
    public string? FullName { get; set; }

    /// <summary>
    /// 年齡
    /// </summary>
    [Range(1, 150)]
    public int? Age { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool? IsActive { get; set; }
}