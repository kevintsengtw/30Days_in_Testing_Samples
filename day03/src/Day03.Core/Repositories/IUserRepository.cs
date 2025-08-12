namespace Day03.Core.Repositories;

/// <summary>
/// interface IUserRepository - 表示用於管理使用者資料的儲存庫。
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 從儲存庫中檢索所有使用者。
    /// </summary>
    /// <returns>所有使用者的可列舉集合。</returns>
    IEnumerable<User> GetAllUsers();

    /// <summary>
    /// 根據電子郵件地址檢索使用者。
    /// </summary>
    /// <param name="email">要檢索的使用者的電子郵件地址。</param>
    /// <returns>具有指定電子郵件地址的使用者，若不存在則返回 <c>null</c>。</returns>
    User? GetUserByEmail(string email);

    /// <summary>
    /// 在儲存庫中建立新使用者。
    /// </summary>
    /// <param name="user">要建立的使用者。</param>
    /// <returns>已建立的使用者。</returns>
    User CreateUser(User user);
}