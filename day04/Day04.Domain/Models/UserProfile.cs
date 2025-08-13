namespace Day04.Domain.Models;

/// <summary>
/// class UserProfile - 用於表示用戶的個人資料。
/// </summary>
public class UserProfile
{
    /// <summary>
    /// 用戶年齡
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 用戶所在城市
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 用戶電話
    /// </summary>
    public string? Phone { get; set; }
}