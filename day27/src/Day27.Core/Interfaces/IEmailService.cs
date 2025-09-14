namespace Day27.Core.Interfaces;

/// <summary>
/// 電子郵件服務介面
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// 發送歡迎郵件
    /// </summary>
    /// <param name="email">收件者電子郵件</param>
    /// <param name="userName">使用者姓名</param>
    /// <returns>是否發送成功</returns>
    Task<bool> SendWelcomeEmailAsync(string email, string userName);

    /// <summary>
    /// 發送密碼重設郵件
    /// </summary>
    /// <param name="email">收件者電子郵件</param>
    /// <param name="resetToken">重設令牌</param>
    /// <returns>是否發送成功</returns>
    Task<bool> SendPasswordResetEmailAsync(string email, string resetToken);
}