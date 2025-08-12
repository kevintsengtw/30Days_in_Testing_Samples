namespace Day03.Core.Services;

/// <summary>
/// class UserService - 用戶服務實現
/// </summary>
public class UserService : IUserService
{
    private readonly List<User> _users = [];
    private int _nextId = 1;

    /// <summary>
    /// 創建用戶
    /// </summary>
    /// <param name="user">用戶</param>
    public User CreateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            throw new ArgumentException("用戶名稱不能為空", nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("用戶電子郵件不能為空", nameof(user));
        }

        var emailValidator = new EmailValidator();
        if (!emailValidator.IsValidEmail(user.Email))
        {
            throw new ArgumentException("電子郵件格式無效", nameof(user));
        }

        var newUser = new User
        {
            Id = this._nextId++,
            Name = user.Name,
            Email = user.Email,
            Age = user.Age,
            Roles = user.Roles,
            Settings = user.Settings,
            CreatedAt = DateTime.UtcNow
        };

        this._users.Add(newUser);
        return newUser;
    }

    /// <summary>
    /// 獲取所有用戶
    /// </summary>
    public IEnumerable<User> GetAllUsers()
    {
        return this._users.AsReadOnly();
    }

    /// <summary>
    /// 根據電子郵件獲取用戶
    /// </summary>
    /// <param name="email">用戶電子郵件</param>
    public User? GetUserByEmail(string email)
    {
        return this._users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}