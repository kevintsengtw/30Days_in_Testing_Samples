using Day20.Core.Models;

namespace Day20.Core.Services;

/// <summary>
/// 使用者服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 取得所有使用者
    /// </summary>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// 根據 ID 取得使用者
    /// </summary>
    Task<User?> GetUserByIdAsync(string id);

    /// <summary>
    /// 根據使用者名稱取得使用者
    /// </summary>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// 建立新使用者
    /// </summary>
    Task<User> CreateUserAsync(UserCreateRequest request);

    /// <summary>
    /// 更新使用者
    /// </summary>
    Task<User?> UpdateUserAsync(string id, UserUpdateRequest request);

    /// <summary>
    /// 刪除使用者
    /// </summary>
    Task<bool> DeleteUserAsync(string id);

    /// <summary>
    /// 檢查使用者是否存在
    /// </summary>
    Task<bool> UserExistsAsync(string id);

    /// <summary>
    /// 檢查使用者名稱是否已存在
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);

    /// <summary>
    /// 記錄使用者登入
    /// </summary>
    Task UpdateLastLoginAsync(string id);

    /// <summary>
    /// 取得所有啟用的使用者
    /// </summary>
    Task<List<User>> GetActiveUsersAsync();

    /// <summary>
    /// 建立使用者 (簡化版本)
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <param name="email">電子郵件</param>
    /// <returns>建立的使用者</returns>
    Task<User> CreateUserAsync(string username, string email);
}