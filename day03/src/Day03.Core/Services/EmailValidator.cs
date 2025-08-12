using System.Text.RegularExpressions;

namespace Day03.Core.Services;

/// <summary>
/// class EmailValidator - 用於驗證電子郵件地址的服務。
/// </summary>
public class EmailValidator
{
    /// <summary>
    /// 驗證電子郵件地址的格式是否有效。
    /// </summary>
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// 驗證電子郵件地址的格式是否有效。
    /// </summary>
    /// <param name="email">要驗證的電子郵件地址。</param>
    /// <returns>如果電子郵件地址有效，則為 <c>true</c>，否則為 <c>false</c>。</returns>
    public bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return email.Length <= 64 && EmailRegex.IsMatch(email);
    }
}