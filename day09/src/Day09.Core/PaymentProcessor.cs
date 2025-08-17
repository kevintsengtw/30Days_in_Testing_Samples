using Day09.Core.Models;

namespace Day09.Core;

/// <summary>
/// class PaymentProcessor - 付款處理器
/// </summary>
public class PaymentProcessor
{
    /// <summary>
    /// 處理付款
    /// </summary>
    /// <param name="request">付款請求</param>
    /// <returns>付款結果</returns>
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        if (!ValidateRequest(request))
        {
            return PaymentResult.Failed("Invalid request");
        }

        var fee = CalculateFee(request.Amount, request.PaymentMethod);
        var total = request.Amount + fee;

        return PaymentResult.Success(total);
    }

    /// <summary>
    /// 驗證請求（私有方法，包含複雜邏輯）
    /// </summary>
    /// <param name="request">付款請求</param>
    /// <returns>是否有效</returns>
    private bool ValidateRequest(PaymentRequest request)
    {
        return request is { Amount: > 0 };
    }

    /// <summary>
    /// 計算手續費（私有方法，包含複雜邏輯）
    /// </summary>
    /// <param name="amount">金額</param>
    /// <param name="method">付款方式</param>
    /// <returns>手續費</returns>
    private decimal CalculateFee(decimal amount, PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.CreditCard => amount * 0.03m,
            PaymentMethod.DebitCard => amount * 0.01m,
            PaymentMethod.BankTransfer => Math.Max(amount * 0.005m, 10m),
            _ => 0m
        };
    }

    /// <summary>
    /// 檢查是否為工作日（靜態私有方法）
    /// </summary>
    /// <param name="date">日期</param>
    /// <returns>是否為工作日</returns>
    private static bool IsBusinessDay(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday &&
               date.DayOfWeek != DayOfWeek.Sunday;
    }
}