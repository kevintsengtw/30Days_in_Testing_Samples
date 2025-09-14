namespace Day27.Core.Interfaces;

/// <summary>
/// 通知服務介面
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// 發送訂單確認通知
    /// </summary>
    /// <param name="customerId">客戶識別碼</param>
    /// <param name="orderId">訂單識別碼</param>
    /// <returns>是否發送成功</returns>
    Task<bool> SendOrderConfirmationAsync(int customerId, string orderId);
}