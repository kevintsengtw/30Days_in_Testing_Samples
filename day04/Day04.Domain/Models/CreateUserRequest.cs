namespace Day04.Domain.Models;

/// <summary>
/// class CreateUserRequest - 用於創建使用者的請求模型。
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// 使用者電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 使用者年齡
    /// </summary>
    public int Age { get; set; }
}