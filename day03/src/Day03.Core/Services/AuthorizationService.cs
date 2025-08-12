namespace Day03.Core.Services;

/// <summary>
/// class AuthorizationService - 用於處理使用者授權邏輯的服務。
/// </summary>
public class AuthorizationService
{
    /// <summary>
    /// 檢查使用者是否可以訪問管理員功能。
    /// </summary>
    /// <param name="user">要檢查的使用者。</param>
    /// <returns>如果使用者可以訪問管理員功能，則為 <c>true</c>，否則為 <c>false</c>。</returns>
    public bool CanAccessAdminFeatures(User user)
    {
        return user.Roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);
    }
}