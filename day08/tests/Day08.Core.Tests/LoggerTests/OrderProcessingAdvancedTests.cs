using Day08.Core.Interface;
using Day08.Core.Logging;
using Day08.Core.Models;
using Day08.Core.Services;
using Day08.Core.Tests.Logging;
using Xunit.Abstractions;

namespace Day08.Core.Tests.LoggerTests;

/// <summary>
/// 使用 CompositeLogger 的進階測試
/// </summary>
public class OrderProcessingAdvancedTests
{
    private readonly AbstractLogger<OrderProcessingService> _mockLogger;
    private readonly ITestOutputHelper _output;
    private readonly ILogger<OrderProcessingService> _compositeLogger;
    
    public OrderProcessingAdvancedTests(ITestOutputHelper testOutputHelper)
    {
        this._output = testOutputHelper;
        this._mockLogger = Substitute.For<AbstractLogger<OrderProcessingService>>();

        var xunitLogger = new XUnitLogger<OrderProcessingService>(testOutputHelper, new LoggerExternalScopeProvider());
        this._compositeLogger = new CompositeLogger<OrderProcessingService>(this._mockLogger, xunitLogger);
    }

    [Fact]
    public void ProcessOrder_付款失敗_應記錄錯誤並輸出到測試結果()
    {
        // Arrange
        var inventoryService = Substitute.For<IInventoryService>();
        var paymentService = Substitute.For<IPaymentService>();

        var order = new Order
        {
            Id = "ORD004",
            ProductId = "PROD004",
            Quantity = 1,
            TotalAmount = 2000
        };

        inventoryService.CheckStock(order.ProductId, order.Quantity).Returns(true);
        paymentService.ProcessPayment(order.TotalAmount)
                      .Returns(new PaymentResult { Success = false, ErrorMessage = "餘額不足" });

        var sut = new OrderProcessingService(this._compositeLogger, inventoryService, paymentService);

        // Act
        var result = sut.ProcessOrder(order);

        // Assert
        result.Success.Should().BeFalse();

        // 驗證 Mock Logger（行為驗證）
        this._mockLogger.Received().Log(
            logLevel: LogLevel.Error,
            ex: null,
            information: Arg.Is<string>(msg => msg.Contains("付款失敗") && msg.Contains("ORD004")));

        // XUnit Logger 會自動將訊息輸出到測試結果中，方便除錯
        // 輸出格式：[HH:mm:ss.fff] [Error] [OrderProcessingService] 訂單 ORD004 付款失敗：餘額不足
    }
}