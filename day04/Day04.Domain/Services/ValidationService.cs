namespace Day04.Domain.Services;

/// <summary>
/// class ValidationService - 用於驗證輸入的服務。
/// </summary>
public class ValidationService
{
    /// <summary>
    /// 驗證電子郵件格式
    /// </summary>
    /// <param name="email">電子郵件地址</param>
    /// <returns>是否有效</returns>
    public bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("電子郵件不能為空", nameof(email));
        }

        if (!email.Contains('@'))
        {
            throw new ArgumentException("電子郵件必須包含 @ 符號", nameof(email));
        }

        return true;
    }
}