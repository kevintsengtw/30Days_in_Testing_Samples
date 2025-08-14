namespace Day05.Domain.Models;

/// <summary>
/// class UpdateUserRequest - 更新用戶請求模型。
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// 用戶名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 用戶電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;
}