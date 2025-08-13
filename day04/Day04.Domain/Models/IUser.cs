namespace Day04.Domain.Models;

/// <summary>
/// interface IUser - 用於表示用戶的接口。
/// </summary>
public interface IUser
{
    /// <summary>
    /// 用戶ID
    /// </summary>
    /// <value></value>
    int Id { get; set; }

    /// <summary>
    /// 用戶名稱
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 用戶電子郵件
    /// </summary>
    string Email { get; set; }

    /// <summary>
    /// 用戶是否活躍
    /// </summary>
    bool IsActive { get; set; }
}