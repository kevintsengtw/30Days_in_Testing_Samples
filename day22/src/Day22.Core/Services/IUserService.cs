using Day22.Core.Models.Mongo;

namespace Day22.Core.Services;

/// <summary>
/// 使用者服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 新增使用者
    /// </summary>
    /// <param name="user">使用者資料</param>
    /// <returns>建立的使用者</returns>
    Task<UserDocument> CreateUserAsync(UserDocument user);

    /// <summary>
    /// 根據 ID 取得使用者
    /// </summary>
    /// <param name="id">使用者 ID</param>
    /// <returns>使用者資料</returns>
    Task<UserDocument?> GetUserByIdAsync(string id);

    /// <summary>
    /// 根據電子郵件取得使用者
    /// </summary>
    /// <param name="email">電子郵件地址</param>
    /// <returns>使用者資料</returns>
    Task<UserDocument?> GetUserByEmailAsync(string email);

    /// <summary>
    /// 取得所有使用者
    /// </summary>
    /// <param name="skip">跳過筆數</param>
    /// <param name="limit">限制筆數</param>
    /// <returns>使用者列表</returns>
    Task<List<UserDocument>> GetAllUsersAsync(int skip = 0, int limit = 100);

    /// <summary>
    /// 更新使用者
    /// </summary>
    /// <param name="user">使用者資料</param>
    /// <returns>更新後的使用者</returns>
    Task<UserDocument?> UpdateUserAsync(UserDocument user);

    /// <summary>
    /// 刪除使用者
    /// </summary>
    /// <param name="id">使用者 ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteUserAsync(string id);

    /// <summary>
    /// 批次新增使用者
    /// </summary>
    /// <param name="users">使用者列表</param>
    /// <returns>成功建立的使用者數量</returns>
    Task<int> CreateUsersAsync(IEnumerable<UserDocument> users);

    /// <summary>
    /// 取得使用者總數
    /// </summary>
    /// <returns>使用者總數</returns>
    Task<long> GetUserCountAsync();

    /// <summary>
    /// 檢查使用者是否存在
    /// </summary>
    /// <param name="email">電子郵件地址</param>
    /// <returns>是否存在</returns>
    Task<bool> UserExistsAsync(string email);
}