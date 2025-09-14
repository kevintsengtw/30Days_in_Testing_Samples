using Day27.Core.Models;

namespace Day27.Core.Interfaces;

/// <summary>
/// 使用者倉庫介面
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根據電子郵件檢查使用者是否存在
    /// </summary>
    /// <param name="email">電子郵件地址</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// 儲存使用者
    /// </summary>
    /// <param name="user">使用者實體</param>
    /// <returns>儲存後的使用者實體</returns>
    Task<User> SaveAsync(User user);

    /// <summary>
    /// 根據識別碼取得使用者
    /// </summary>
    /// <param name="id">使用者識別碼</param>
    /// <returns>使用者實體</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// 根據電子郵件取得使用者
    /// </summary>
    /// <param name="email">電子郵件地址</param>
    /// <returns>使用者實體</returns>
    Task<User?> GetByEmailAsync(string email);
}