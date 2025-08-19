namespace Day10.Core.Services;

/// <summary>
/// 使用者
/// </summary>
public class User
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 年齡
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 會員等級
    /// </summary>
    public MemberLevel MemberLevel { get; set; }

    /// <summary>
    /// 總消費金額
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// 會員年數
    /// </summary>
    public int MembershipYears { get; set; }

    /// <summary>
    /// 內部編號
    /// </summary>
    public string? InternalId { get; set; }
}