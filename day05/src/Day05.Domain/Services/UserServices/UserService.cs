using Day05.Domain.DomainModels;
using Day05.Domain.Models;

namespace Day05.Domain.Services.UserServices;

/// <summary>
/// class UserService - 用戶服務
/// </summary>
public class UserService
{
    /// <summary>
    /// 創建用戶
    /// </summary>
    /// <param name="email">用戶電子郵件</param>
    /// <returns>創建的用戶</returns>
    public User CreateUser(string email)
    {
        return new User
        {
            Id = Random.Shared.Next(1, 1000),
            Name = "Test User",
            Email = email,
            Role = "user",
            Permissions = ["READ"],
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            RowVersion = [1, 2, 3, 4]
        };
    }

    /// <summary>
    /// 獲取用戶
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <returns>用戶信息</returns>
    public User GetUser(int id)
    {
        return new User
        {
            Id = id,
            Name = "Test User",
            Email = "test@example.com",
            Role = "user",
            Permissions = ["READ"],
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now,
            RowVersion = [1, 2, 3, 5]
        };
    }

    /// <summary>
    /// 處理用戶
    /// </summary>
    /// <param name="isAdmin">是否為管理員</param>
    /// <returns>處理後的用戶</returns>
    public User ProcessUser(bool isAdmin)
    {
        return new User
        {
            Id = 1,
            Name = "Test User",
            Email = "test@example.com",
            Role = isAdmin ? "admin" : "user",
            Permissions = isAdmin ? ["READ", "WRITE", "DELETE"] : ["READ"],
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }

    /// <summary>
    /// 註冊用戶
    /// </summary>
    /// <param name="profile">用戶資料</param>
    /// <returns>註冊結果</returns>
    public UserRegistration RegisterUser(UserProfile profile)
    {
        return new UserRegistration
        {
            UserId = Random.Shared.Next(1, 1000),
            Email = "test@example.com",
            IsEmailVerified = false
        };
    }

    /// <summary>
    /// 處理大量用戶批次
    /// </summary>
    /// <param name="users">用戶列表</param>
    /// <returns>處理後的用戶列表</returns>
    public List<User> ProcessLargeUserBatch(List<User> users)
    {
        return users.Select(u => new User
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role,
            Permissions = u.Permissions,
            CreatedAt = u.CreatedAt,
            UpdatedAt = DateTime.Now
        }).ToList();
    }
}