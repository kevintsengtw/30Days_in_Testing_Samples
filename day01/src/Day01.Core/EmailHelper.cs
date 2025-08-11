using System.Text.RegularExpressions;

namespace Day01.Core;

/// <summary>
/// class EmailHelper - Email 相關的輔助工具類別
/// </summary>
public class EmailHelper
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// 驗證 Email 格式是否有效
    /// </summary>
    /// <param name="email">要驗證的 Email 字串</param>
    /// <returns>如果格式有效回傳 true，否則回傳 false</returns>
    public bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailRegex.IsMatch(email);
    }

    /// <summary>
    /// 從 Email 地址中提取網域名稱
    /// </summary>
    /// <param name="email">Email 地址</param>
    /// <returns>網域名稱，如果格式無效則回傳 null</returns>
    public string? GetDomain(string? email)
    {
        if (!this.IsValidEmail(email))
        {
            return null;
        }

        var atIndex = email!.IndexOf('@');
        return email.Substring(atIndex + 1);
    }
}