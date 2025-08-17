using Day08.Core.Interface;
using Day08.Core.Models;
using Day08.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Day08.Core.Tests.Integration;

/// <summary>
/// 訂單處理整合測試
/// </summary>
public class OrderProcessingIntegrationTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ITestOutputHelper _output;

    public OrderProcessingIntegrationTests(ITestOutputHelper testOutputHelper)
    {
        this._output = testOutputHelper;

        var services = new ServiceCollection();

        // 設置測試用的 Logger
        services.AddLogging(builder =>
        {
            builder.AddProvider(new XUnitLoggerProvider(testOutputHelper));
        });

        // 註冊業務服務
        services.AddScoped<IOrderRepository, InMemoryOrderRepository>();
        services.AddScoped<IPaymentService, MockPaymentService>();
        services.AddScoped<OrderProcessor>();

        this._serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task ProcessOrderAsync_完整訂單流程_應記錄所有步驟()
    {
        // Arrange
        using var scope = this._serviceProvider.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<OrderProcessor>();

        var order = new Order
        {
            Id = $"ORDER-{Guid.NewGuid().ToString("N")[..8]}",
            CustomerId = "CUST001",
            Items =
            [
                new OrderItem { ProductId = "P001", ProductName = "測試商品", Quantity = 2, Price = 100 }
            ]
        };

        this._output.WriteLine("=== 測試訂單處理流程 ===");
        this._output.WriteLine($"訂單編號: {order.Id}");
        this._output.WriteLine($"客戶編號: {order.CustomerId}");

        // Act
        var result = await processor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.OrderId.Should().Be(order.Id);
        result.TotalAmount.Should().Be(200); // 2 * 100

        this._output.WriteLine("=== 測試完成 ===");
        this._output.WriteLine($"處理結果: {(result.Success ? "成功" : "失敗")}");
        this._output.WriteLine($"訂單金額: {result.TotalAmount:C}");

        // Logger 輸出會顯示在測試結果中，包含完整的處理流程
    }

    [Fact]
    public async Task ProcessOrderAsync_空訂單項目_應正確處理()
    {
        // Arrange
        using var scope = this._serviceProvider.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<OrderProcessor>();

        var order = new Order
        {
            Id = "EMPTY-" + Guid.NewGuid().ToString("N")[..8],
            CustomerId = "CUST002",
            Items = Array.Empty<OrderItem>()
        };

        this._output.WriteLine($"=== 測試空訂單處理 ===");
        this._output.WriteLine($"訂單編號: {order.Id}");

        // Act
        var result = await processor.ProcessOrderAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.TotalAmount.Should().Be(0);

        this._output.WriteLine($"=== 測試完成 ===");
        this._output.WriteLine($"處理結果: 成功");
        this._output.WriteLine($"訂單金額: ${result.TotalAmount}");
    }
}

/// <summary>
/// class InMemoryOrderRepository - 記憶體中的訂單儲存庫實作
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ILogger<InMemoryOrderRepository>? _logger;
    private readonly Dictionary<string, Order> _orders = new();

    /// <summary>
    /// InMemoryOrderRepository 建構子
    /// </summary>
    /// <param name="logger">The logger.</param>
    public InMemoryOrderRepository(ILogger<InMemoryOrderRepository>? logger = null)
    {
        this._logger = logger;
    }

    /// <summary>
    /// 儲存訂單
    /// </summary>
    /// <param name="order">The order.</param>
    /// <returns></returns>
    public async Task<bool> SaveOrderAsync(Order order)
    {
        this._logger?.LogInformation("儲存訂單 {OrderId}", order.Id);

        await Task.Delay(50); // 模擬 I/O 操作

        this._orders[order.Id] = order;

        this._logger?.LogInformation("訂單 {OrderId} 儲存成功", order.Id);
        return true;
    }

    /// <summary>
    /// 根據訂單 ID 查詢訂單
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        this._logger?.LogInformation("查詢訂單 {OrderId}", orderId);

        await Task.Delay(30); // 模擬 I/O 操作

        var found = this._orders.TryGetValue(orderId, out var order);

        if (found)
        {
            this._logger?.LogInformation("找到訂單 {OrderId}", orderId);
        }
        else
        {
            this._logger?.LogWarning("訂單 {OrderId} 不存在", orderId);
        }

        return order;
    }
}

/// <summary>
/// class MockPaymentService - Mock 付款服務實作
/// </summary>
public class MockPaymentService : IPaymentService
{
    private readonly ILogger<MockPaymentService>? _logger;

    /// <summary>
    /// MockPaymentService 建構子
    /// </summary>
    /// <param name="logger">The logger.</param>
    public MockPaymentService(ILogger<MockPaymentService>? logger = null)
    {
        this._logger = logger;
    }

    /// <summary>
    /// 處理付款
    /// </summary>
    /// <param name="amount">付款金額</param>
    /// <returns>付款結果</returns>
    public PaymentResult ProcessPayment(decimal amount)
    {
        this._logger?.LogInformation("處理付款，金額：${Amount}", amount);

        // 模擬成功付款
        var result = new PaymentResult
        {
            Success = true,
            TransactionId = Guid.NewGuid().ToString("N")[..8]
        };

        this._logger?.LogInformation("付款成功，交易編號：{TransactionId}", result.TransactionId);
        return result;
    }

    /// <summary>
    /// 處理付款
    /// </summary>
    /// <param name="request">付款請求</param>
    /// <returns>付款結果</returns>
    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        return ProcessPayment(request.Amount);
    }
}