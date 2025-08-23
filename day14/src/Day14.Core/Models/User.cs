namespace Day14.Core.Models;

/// <summary>
/// 使用者資訊
/// </summary>
public class User
{
    /// <summary>
    /// 使用者編號
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 姓
    /// </summary>
    public string FirstName { get; set; } = "";

    /// <summary>
    /// 名
    /// </summary>
    public string LastName { get; set; } = "";

    /// <summary>
    /// 全名
    /// </summary>
    public string FullName => $"{this.FirstName} {this.LastName}";

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// 電話號碼
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 年齡
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 是否為 Premium 用戶
    /// </summary>
    public bool IsPremium { get; set; }

    /// <summary>
    /// 積分
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// 暱稱
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string Department { get; set; } = "";

    /// <summary>
    /// 角色
    /// </summary>
    public string Role { get; set; } = "";

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}