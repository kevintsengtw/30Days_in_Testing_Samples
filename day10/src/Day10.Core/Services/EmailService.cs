namespace Day10.Core.Services;

/// <summary>
/// 電子郵件服務
/// </summary>
public class EmailService
{
    /// <summary>
    /// 發送電子郵件
    /// </summary>
    public bool SendEmail(string email, string subject, string body)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            return false;
        }

        if (string.IsNullOrEmpty(subject))
        {
            return false;
        }

        // 模擬發送邏輯
        return true;
    }
}