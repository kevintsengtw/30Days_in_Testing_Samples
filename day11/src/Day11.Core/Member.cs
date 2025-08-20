namespace Day11.Core;

/// <summary>
/// class Member - 會員類別，用於展示屬性值範圍控制
/// </summary>
public class Member
{
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 年齡，限制範圍為 10-80
    /// </summary>
    [Range(10, 80)]
    public int Age { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdateTime { get; set; }
}