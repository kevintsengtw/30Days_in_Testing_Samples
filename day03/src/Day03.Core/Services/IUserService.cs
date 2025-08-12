namespace Day03.Core.Services;

/// <summary>
/// interface IUserService - 用戶服務
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 創建用戶
    /// </summary>
    /// <param name="user">用戶</param>
    User CreateUser(User user);

    /// <summary>
    /// 獲取所有用戶
    /// </summary>
    IEnumerable<User> GetAllUsers();

    /// <summary>
    /// 根據電子郵件獲取用戶
    /// </summary>
    /// <param name="email">用戶電子郵件</param>
    User? GetUserByEmail(string email);
}