using Day20.Core.Data;
using Day20.Core.Models;

namespace Day20.Core.Services;

/// <summary>
/// SQL 資料庫版本的使用者服務
/// </summary>
public class SqlUserService : IUserService
{
    private readonly UserDbContext _context;

    public SqlUserService(UserDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 取得所有使用者
    /// </summary>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
                             .OrderBy(u => u.CreatedAt)
                             .ToListAsync();
    }

    /// <summary>
    /// 根據 ID 取得使用者
    /// </summary>
    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await _context.Users
                             .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// 根據使用者名稱取得使用者
    /// </summary>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
                             .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// 建立新使用者
    /// </summary>
    public async Task<User> CreateUserAsync(UserCreateRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            Age = request.Age,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// 更新使用者
    /// </summary>
    public async Task<User?> UpdateUserAsync(string id, UserUpdateRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.FullName))
        {
            user.FullName = request.FullName;
        }

        if (request.Age.HasValue)
        {
            user.Age = request.Age.Value;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// 刪除使用者
    /// </summary>
    public async Task<bool> DeleteUserAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 檢查使用者是否存在
    /// </summary>
    public async Task<bool> UserExistsAsync(string id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    /// <summary>
    /// 檢查使用者名稱是否已存在
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    /// <summary>
    /// 記錄使用者登入
    /// </summary>
    public async Task UpdateLastLoginAsync(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 取得所有啟用的使用者
    /// </summary>
    public async Task<List<User>> GetActiveUsersAsync()
    {
        return await _context.Users
                             .Where(u => u.IsActive)
                             .OrderBy(u => u.Username)
                             .ToListAsync();
    }

    /// <summary>
    /// 建立使用者 (簡化版本)
    /// </summary>
    /// <param name="username">使用者名稱</param>
    /// <param name="email">電子郵件</param>
    /// <returns>建立的使用者</returns>
    public async Task<User> CreateUserAsync(string username, string email)
    {
        // 檢查使用者名稱是否已存在
        var existingUser = await _context.Users
                                         .FirstOrDefaultAsync(u => u.Username == username);

        if (existingUser != null)
        {
            throw new InvalidOperationException($"使用者名稱 '{username}' 已存在");
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email,
            FullName = username, // 預設使用 username 作為 FullName
            Age = 18,            // 預設年齡
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}