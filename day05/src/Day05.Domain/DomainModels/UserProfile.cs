namespace Day05.Domain.DomainModels;

/// <summary>
/// class UserProfile - 使用者個人資料
/// </summary>
public class UserProfile
{
    /// <summary>
    /// 名字
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 姓氏
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// 出生日期
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = string.Empty;
}