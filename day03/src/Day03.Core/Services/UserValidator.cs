namespace Day03.Core.Services;

/// <summary>
/// class UserValidator - 用於驗證使用者資料的服務。
/// </summary>
public class UserValidator
{
    /// <summary>
    /// 驗證使用者資料是否有效。
    /// </summary>
    /// <param name="user">要驗證的使用者資料。</param>
    /// <returns>如果使用者資料有效，則為 <c>true</c>，否則為 <c>false</c>。</returns>
    public bool IsValid(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            return false;
        }

        if (user.Age is < 18 or > 120)
        {
            return false;
        }

        var emailValidator = new EmailValidator();
        return emailValidator.IsValidEmail(user.Email);
    }
}