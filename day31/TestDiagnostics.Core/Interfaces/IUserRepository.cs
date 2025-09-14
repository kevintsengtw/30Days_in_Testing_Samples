/// <summary>
/// 使用者資料存取介面
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 取得所有使用者
    /// </summary>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// 根據識別碼取得使用者
    /// </summary>
    Task<User?> GetUserByIdAsync(string id);

    /// <summary>
    /// 儲存使用者
    /// </summary>
    Task SaveUserAsync(User user);
}