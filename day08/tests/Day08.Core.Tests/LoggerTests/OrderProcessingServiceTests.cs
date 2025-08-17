using Day08.Core.Interface;
using Day08.Core.Logging;
using Day08.Core.Models;
using Day08.Core.Services;

namespace Day08.Core.Tests.LoggerTests;

/// <summary>
/// 使用 AbstractLogger 的訂單處理服務測試
/// </summary>
public class OrderProcessingServiceTests
{
    private readonly AbstractLogger<OrderProcessingService> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// 訂單處理服務測試
    /// </summary>
    public OrderProcessingServiceTests()
    {
        this._logger = Substitute.For<AbstractLogger<OrderProcessingService>>();
        this._inventoryService = Substitute.For<IInventoryService>();
        this._paymentService = Substitute.For<IPaymentService>();
    }

    [Fact]
    public void ProcessOrder_正常處理_應記錄開始與完成訊息()
    {
        // Arrange
        var order = new Order
        {
            Id = "ORD001",
            CustomerId = "CUST001",
            ProductId = "PROD001",
            Quantity = 2,
            TotalAmount = 1000
        };

        this._inventoryService.CheckStock(order.ProductId, order.Quantity).Returns(true);
        this._paymentService.ProcessPayment(order.TotalAmount)
            .Returns(new PaymentResult { Success = true });

        var sut = new OrderProcessingService(this._logger, this._inventoryService, this._paymentService);

        // Act
        var result = sut.ProcessOrder(order);

        // Assert
        result.Success.Should().BeTrue();

        // 驗證記錄了開始處理訊息
        this._logger.Received().Log(
            logLevel: LogLevel.Information,
            ex: null,
            information: Arg.Is<string>(msg => msg.Contains("開始處理訂單") && msg.Contains("ORD001")));

        // 驗證記錄了完成訊息
        this._logger.Received().Log(
            logLevel: LogLevel.Information,
            ex: null,
            information: Arg.Is<string>(msg => msg.Contains("處理完成") && msg.Contains("1000")));
    }

    [Fact]
    public void ProcessOrder_庫存不足_應記錄警告訊息()
    {
        // Arrange
        var order = new Order
        {
            Id = "ORD002",
            ProductId = "PROD002",
            Quantity = 5
        };

        this._inventoryService.CheckStock(order.ProductId, order.Quantity).Returns(false);

        var sut = new OrderProcessingService(this._logger, this._inventoryService, this._paymentService);

        // Act
        var result = sut.ProcessOrder(order);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("庫存不足");

        this._logger.Received().Log(
            logLevel: LogLevel.Warning,
            ex: null,
            information: Arg.Is<string>(msg => msg.Contains("庫存不足") && msg.Contains("PROD002")));
    }

    [Fact]
    public void ProcessOrder_付款失敗_應記錄錯誤訊息()
    {
        // Arrange
        var order = new Order
        {
            Id = "ORD003",
            ProductId = "PROD003",
            Quantity = 1,
            TotalAmount = 500
        };

        this._inventoryService.CheckStock(order.ProductId, order.Quantity).Returns(true);
        this._paymentService.ProcessPayment(order.TotalAmount)
            .Returns(new PaymentResult { Success = false, ErrorMessage = "信用卡驗證失敗" });

        var sut = new OrderProcessingService(this._logger, this._inventoryService, this._paymentService);

        // Act
        var result = sut.ProcessOrder(order);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("付款失敗");

        this._logger.Received().Log(
            logLevel: LogLevel.Error,
            ex: null,
            information: Arg.Is<string>(msg => msg.Contains("付款失敗") && msg.Contains("ORD003")));
    }
}