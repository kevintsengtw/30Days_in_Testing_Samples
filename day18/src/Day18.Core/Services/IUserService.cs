namespace Day18.Core.Services;

/// <summary>
/// interface IUserService - 用戶服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 檢查使用者名稱是否可用
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <returns>是否可用</returns>
    Task<bool> IsUsernameAvailableAsync(string username);

    /// <summary>
    /// 檢查電子郵件是否已註冊
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否已註冊</returns>
    Task<bool> IsEmailRegisteredAsync(string email);
}