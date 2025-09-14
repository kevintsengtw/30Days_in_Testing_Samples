namespace Day27.Core.Models;

/// <summary>
/// 使用者註冊請求
/// </summary>
public class RegisterUserRequest
{
    /// <summary>
    /// 電子郵件地址
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者姓名
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}