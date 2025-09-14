/// <summary>
/// 改良版的使用者服務 - 修正了靜態狀態問題
/// </summary>
public class ImprovedUserService
{
    private readonly IUserRepository _repository;
    private int _instanceCounter = 0; // 改為實例欄位

    public ImprovedUserService(IUserRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 取得使用者數量
    /// </summary>
    public async Task<int> GetUserCountAsync()
    {
        _instanceCounter++; // 每個測試實例都有獨立狀態
        var users = await _repository.GetAllUsersAsync();
        return users.Count();
    }

    /// <summary>
    /// 取得活躍使用者
    /// </summary>
    public async Task<List<User>> GetActiveUsersAsync()
    {
        var allUsers = await _repository.GetAllUsersAsync();
        return allUsers.Where(u => u.IsActive).ToList();
    }

    /// <summary>
    /// 驗證使用者
    /// </summary>
    public async Task<bool> ValidateUserAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return false;

        var user = await _repository.GetUserByIdAsync(userId);
        return user != null && user.IsActive;
    }

    /// <summary>
    /// 改良版的登入驗證 - 使用參數而非 DateTime.Now
    /// </summary>
    public async Task<bool> IsUserLoginValidAsync(string userId, DateTime currentTime)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user == null) return false;

        // 使用傳入的時間參數，避免不確定性
        var timeDiff = currentTime - user.LastLoginTime;
        return timeDiff.TotalHours < 24;
    }
}
