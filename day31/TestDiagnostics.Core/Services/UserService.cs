/// <summary>
/// 使用者服務
/// </summary>
public class UserService
{
    private readonly IUserRepository _repository;
    private static int _staticCounter = 0; // 故意設計的靜態狀態問題

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 取得使用者數量
    /// </summary>
    public async Task<int> GetUserCountAsync()
    {
        _staticCounter++; // 這會造成測試間的狀態污染
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
    /// 模擬時間相關的處理
    /// </summary>
    public async Task<bool> IsUserLoginValidAsync(string userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user == null) return false;

        // 故意使用 DateTime.Now 造成不確定性
        var timeDiff = DateTime.Now - user.LastLoginTime;
        return timeDiff.TotalHours < 24;
    }
}
