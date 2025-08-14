using Day05.Domain.DomainModels;

namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class PaymentService - 付款服務
/// </summary>
public class PaymentService
{
    /// <summary>
    /// 處理付款請求
    /// </summary>
    /// <param name="request">付款請求</param>
    /// <returns>付款結果</returns>
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        if (request.Amount <= 0)
        {
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorMessage = "付款金額無效",
                ErrorCode = "INVALID_AMOUNT"
            };
        }

        return new PaymentResult
        {
            IsSuccess = true,
            TransactionId = Guid.NewGuid().ToString()
        };
    }
}