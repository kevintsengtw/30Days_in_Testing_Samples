namespace Day18.Core.Models;

/// <summary>
/// class UserRegistrationRequest - 使用者註冊請求資料
/// </summary>
public class UserRegistrationRequest
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件地址
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 出生日期
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// 年齡
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 電話號碼
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 使用者角色清單
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// 是否同意使用條款
    /// </summary>
    public bool AgreeToTerms { get; set; }
}