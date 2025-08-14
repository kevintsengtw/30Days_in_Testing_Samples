namespace Day05.Domain.DomainModels;

/// <summary>
/// class User - 使用者
/// </summary>
public class User
{
    /// <summary>
    /// 使用者識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 使用者電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者角色
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// 使用者權限
    /// </summary>
    public string[] Permissions { get; set; } = [];

    /// <summary>
    /// 使用者建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 使用者更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 使用者資料行版本
    /// </summary>
    public byte[] RowVersion { get; set; } = [];

    /// <summary>
    /// 使用者個人資料
    /// </summary>
    public UserProfile? Profile { get; set; }
}