using Day27.Core.Models;

namespace Day27.Core.Interfaces;

/// <summary>
/// 付款服務介面
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// 處理付款
    /// </summary>
    /// <param name="amount">付款金額</param>
    /// <param name="paymentMethod">付款方式</param>
    /// <returns>付款結果</returns>
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string paymentMethod);
}