namespace Day03.Core.Models;

/// <summary>
/// class User - 代表使用者實體。
/// </summary>
public class User
{
    /// <summary>
    /// 使用者的唯一識別碼。
    /// </summary>
    /// <value></value>
    public int Id { get; set; }

    /// <summary>
    /// 使用者名稱。
    /// </summary>
    /// <value></value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 使用者電子郵件地址。
    /// </summary>
    /// <value></value>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者年齡。
    /// </summary>
    /// <value></value>
    public int Age { get; set; }

    /// <summary>
    /// 使用者角色陣列。
    /// </summary>
    /// <value></value>
    public string[] Roles { get; set; } = [];

    /// <summary>
    /// 使用者設定。
    /// </summary>
    /// <returns></returns>
    public UserSettings Settings { get; set; } = new();

    /// <summary>
    /// 使用者建立時間。
    /// </summary>
    /// <value></value>
    public DateTime CreatedAt { get; set; }
}