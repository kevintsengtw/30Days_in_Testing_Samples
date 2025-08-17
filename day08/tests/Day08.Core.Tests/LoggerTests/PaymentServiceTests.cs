using Day08.Core.Models;
using Day08.Core.Tests.Logging;

namespace Day08.Core.Tests.LoggerTests;

/// <summary>
/// 結構化記錄測試範例
/// </summary>
public class PaymentServiceTests
{
    [Fact]
    public void ProcessPayment_付款失敗交易_應記錄結構化資料()
    {
        // Arrange
        var paymentRequest = new PaymentRequest
        {
            Amount = 1000,
            Currency = "TWD",
            CardNumber = "****-****-****-1234"
        };

        var mockLogger = new TestLogger<PaymentService>();
        var service = new PaymentService(mockLogger);

        // Act
        var result = service.ProcessPayment(paymentRequest);

        // Assert
        result.Success.Should().BeFalse();

        // 驗證記錄內容
        var errorLogs = mockLogger.GetLogs(LogLevel.Error);
        errorLogs.Count.Should().Be(1);

        var errorLog = errorLogs[0];
        errorLog.Message.Should().Contain("Payment processing failed");
        errorLog.State.Should().ContainKey("Amount");
        errorLog.State.Should().NotContainKey("CardNumber"); // 確保敏感資料未記錄

        // 驗證敏感資料未被記錄
        errorLog.Message.Should().NotContain("1234");
    }
}

/// <summary>
/// class PaymentService - 簡單的付款服務實作（用於測試）
/// </summary>
public class PaymentService
{
    private readonly ILogger<PaymentService>? _logger;

    public PaymentService(ILogger<PaymentService>? logger = null)
    {
        this._logger = logger;
    }

    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        this._logger?.LogInformation("Processing payment for amount {Amount} {Currency}",
                                     request.Amount, request.Currency);

        // 模擬付款失敗
        this._logger?.LogError("Payment processing failed for amount {Amount}", request.Amount);

        return new PaymentResult
        {
            Success = false,
            ErrorMessage = "Insufficient funds"
        };
    }
}