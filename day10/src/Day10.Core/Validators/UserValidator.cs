using Day10.Core.Services;

namespace Day10.Core.Validators;

/// <summary>
/// 使用者驗證器
/// </summary>
public class UserValidator
{
    /// <summary>
    /// 驗證使用者是否有效
    /// </summary>
    public bool IsValid(User user)
    {
        if (user == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(user.Name))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
        {
            return false;
        }

        if (user.Age < 18)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 驗證是否為成年人
    /// </summary>
    public bool IsAdult(User user)
    {
        return user.Age >= 18;
    }
}