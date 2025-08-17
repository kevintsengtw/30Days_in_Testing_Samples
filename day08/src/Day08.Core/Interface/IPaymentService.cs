using Day08.Core.Models;

namespace Day08.Core.Interface;

/// <summary>
/// interface IPaymentService - 付款服務介面
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// 處理付款
    /// </summary>
    /// <param name="amount">付款金額</param>
    /// <returns>付款結果</returns>
    PaymentResult ProcessPayment(decimal amount);

    /// <summary>
    /// 處理付款請求
    /// </summary>
    /// <param name="request">付款請求</param>
    /// <returns>付款結果</returns>
    PaymentResult ProcessPayment(PaymentRequest request);
}