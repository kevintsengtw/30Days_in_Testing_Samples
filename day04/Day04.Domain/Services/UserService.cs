using Day04.Domain.Models;

namespace Day04.Domain.Services;

/// <summary>
/// class UserService - 用於處理用戶的服務。
/// </summary>
public class UserService
{
    private readonly List<User> _users = [];
    private int _nextId = 1;

    /// <summary>
    /// 創建用戶
    /// </summary>
    /// <param name="email">用戶電子郵件</param>
    /// <param name="name">用戶名稱</param>
    /// <returns>創建的用戶</returns>
    public User CreateUser(string email, string name)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("電子郵件不能為空", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("姓名不能為空", nameof(name));
        }

        if (name.Length < 2)
        {
            throw new ArgumentException("姓名至少需要2個字元", nameof(name));
        }

        var user = new User
        {
            Id = this._nextId++,
            Email = email,
            Name = name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        this._users.Add(user);
        return user;
    }

    /// <summary>
    /// 創建用戶
    /// </summary>
    /// <param name="request">用戶創建請求</param>
    /// <returns>創建的用戶</returns>
    public User CreateUser(CreateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return this.CreateUser(request.Email, request.Name);
    }

    /// <summary>
    /// 獲取用戶
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <returns>找到的用戶</returns>
    public User GetUser(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("使用者 ID 必須為正數", nameof(id));
        }

        return this._users.FirstOrDefault(u => u.Id == id)
               ?? throw new InvalidOperationException($"找不到 ID 為 {id} 的使用者");
    }

    /// <summary>
    /// 獲取用戶（非同步）
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <returns>找到的用戶</returns>
    public async Task<User> GetUserAsync(int id)
    {
        // 模擬非同步操作
        await Task.Delay(100);
        return this.GetUser(id);
    }

    /// <summary>
    /// 獲取用戶（帶延遲）
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <param name="cancellationToken">The cancellationToken</param>
    /// <returns>找到的用戶</returns>
    public async Task<User> GetUserWithDelayAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentException("無效的使用者 ID", nameof(id));
        }

        // 模擬長時間操作
        await Task.Delay(200, cancellationToken);
        return GetUser(id);
    }

    /// <summary>
    /// 獲取所有活躍用戶
    /// </summary>
    /// <returns>活躍用戶列表</returns>
    public List<User> GetActiveUsers()
    {
        return this._users.Where(u => u.IsActive).ToList();
    }

    /// <summary>
    /// 清空所有用戶
    /// </summary>
    public void ClearUsers()
    {
        this._users.Clear();
        this._nextId = 1;
    }
}