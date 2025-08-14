namespace Day05.Domain.DomainModels;

/// <summary>
/// class UserEntity - 使用者實體
/// </summary>
public class UserEntity
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
    public int Version { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    public string? LastModifiedBy { get; set; }
}