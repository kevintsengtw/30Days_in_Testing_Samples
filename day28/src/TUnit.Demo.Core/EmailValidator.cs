namespace TUnit.Demo.Core;

/// <summary>
/// 電子郵件驗證器
/// </summary>
public class EmailValidator
{
    /// <summary>
    /// 驗證電子郵件格式是否正確
    /// </summary>
    public bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var parts = email.Split('@');
        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]) || string.IsNullOrWhiteSpace(parts[1]))
        {
            return false;
        }

        return parts[1].Contains('.');
    }

    /// <summary>
    /// 取得電子郵件的網域部分
    /// </summary>
    public string GetDomain(string email)
    {
        if (!IsValidEmail(email))
        {
            throw new ArgumentException("無效的電子郵件格式");
        }

        return email.Split('@')[1];
    }
}