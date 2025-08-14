namespace Day05.Domain.Models;

/// <summary>
/// class UserRegistration - 用戶註冊請求模型。
/// </summary>
public class UserRegistration
{
    /// <summary>
    /// 用戶ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 用戶電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 用戶電子郵件是否已驗證
    /// </summary>
    public bool IsEmailVerified { get; set; }
}