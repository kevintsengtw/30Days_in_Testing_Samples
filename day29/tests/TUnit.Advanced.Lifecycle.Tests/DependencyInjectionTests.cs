namespace TUnit.Advanced.Lifecycle.Tests;

/// <summary>
/// TUnit 依賴注入資料來源屬性
/// 基於 Microsoft.Extensions.DependencyInjection 實作
/// </summary>
public class MicrosoftDependencyInjectionDataSourceAttribute : DependencyInjectionDataSourceAttribute<IServiceScope>
{
    private static readonly IServiceProvider ServiceProvider = CreateSharedServiceProvider();

    public override IServiceScope CreateScope(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return ServiceProvider.CreateScope();
    }

    public override object? Create(IServiceScope scope, Type type)
    {
        return scope.ServiceProvider.GetService(type);
    }

    private static IServiceProvider CreateSharedServiceProvider()
    {
        return new ServiceCollection()
               .AddSingleton<IOrderRepository, MockOrderRepository>()
               .AddSingleton<IDiscountCalculator, MockDiscountCalculator>()
               .AddSingleton<IShippingCalculator, MockShippingCalculator>()
               .AddSingleton<ILogger<OrderService>, MockLogger<OrderService>>()
               .AddTransient<OrderService>()
               .BuildServiceProvider();
    }
}

/// <summary>
/// 展示 TUnit 真正的依賴注入功能
/// </summary>
[MicrosoftDependencyInjectionDataSource]
public class DependencyInjectionTests(OrderService orderService)
{
    [Test]
    public async Task CreateOrder_使用TUnit依賴注入_應正確運作()
    {
        // Arrange - 依賴已經透過 TUnit DI 自動注入
        var items = new List<OrderItem>
        {
            new() { ProductId = "PROD001", ProductName = "測試商品", UnitPrice = 100m, Quantity = 2 }
        };

        // Act
        var order = await orderService.CreateOrderAsync("CUST001", CustomerLevel.VIP會員, items);

        // Assert
        await Assert.That(order).IsNotNull();
        await Assert.That(order.CustomerId).IsEqualTo("CUST001");
        await Assert.That(order.CustomerLevel).IsEqualTo(CustomerLevel.VIP會員);
        await Assert.That(order.Items).HasCount().EqualTo(1);
    }

    [Test]
    public async Task TUnitDependencyInjection_驗證自動注入_服務應為正確類型()
    {
        // Assert - 驗證 TUnit 已正確注入 OrderService 實例
        await Assert.That(orderService).IsNotNull();
        await Assert.That(orderService.GetType().Name).IsEqualTo("OrderService");
    }
}

/// <summary>
/// 展示手動依賴建立的傳統方式（對比用）
/// </summary>
public class ManualDependencyTests
{
    [Test]
    public async Task CreateOrder_手動建立依賴_傳統方式對比()
    {
        // Arrange - 手動建立測試所需的依賴（傳統方式）
        var mockRepository = new MockOrderRepository();
        var mockDiscountCalculator = new MockDiscountCalculator();
        var mockShippingCalculator = new MockShippingCalculator();
        var mockLogger = new MockLogger<OrderService>();

        var orderService = new OrderService(mockRepository, mockDiscountCalculator, mockShippingCalculator, mockLogger);

        var items = new List<OrderItem>
        {
            new() { ProductId = "PROD001", ProductName = "測試商品", UnitPrice = 100m, Quantity = 2 }
        };

        // Act
        var order = await orderService.CreateOrderAsync("CUST001", CustomerLevel.VIP會員, items);

        // Assert
        await Assert.That(order).IsNotNull();
        await Assert.That(order.CustomerId).IsEqualTo("CUST001");
        await Assert.That(order.CustomerLevel).IsEqualTo(CustomerLevel.VIP會員);
        await Assert.That(order.Items).HasCount().EqualTo(1);
    }
}

/// <summary>
/// Mock 實作用於測試
/// </summary>
public class MockOrderRepository : IOrderRepository
{
    public Task<bool> SaveOrderAsync(Order order)
    {
        order.OrderId = Guid.NewGuid().ToString();
        return Task.FromResult(true);
    }

    public Task<Order?> GetOrderByIdAsync(string orderId)
    {
        return Task.FromResult<Order?>(null);
    }

    public Task<bool> UpdateOrderAsync(Order order)
    {
        return Task.FromResult(true);
    }

    public Task<bool> DeleteOrderAsync(string orderId)
    {
        return Task.FromResult(true);
    }

    public Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
    {
        return Task.FromResult(new List<Order>());
    }
}

public class MockDiscountRepository : IDiscountRepository
{
    public Task<DiscountRule?> GetDiscountRuleByCodeAsync(string discountCode)
    {
        return Task.FromResult<DiscountRule?>(null);
    }

    public Task<List<DiscountRule>> GetActiveDiscountRulesAsync()
    {
        return Task.FromResult(new List<DiscountRule>());
    }
}

public class MockDiscountCalculator : IDiscountCalculator
{
    public async Task<decimal> CalculateDiscountAsync(Order order, string discountCode)
    {
        return await Task.FromResult(order.CustomerLevel == CustomerLevel.VIP會員 ? order.TotalAmount * 0.1m : 0m);
    }

    public async Task<bool> ValidateDiscountCodeAsync(string discountCode, CustomerLevel customerLevel, decimal orderAmount)
    {
        return await Task.FromResult(!string.IsNullOrEmpty(discountCode));
    }

    public async Task<DiscountRule?> GetDiscountRuleAsync(string discountCode)
    {
        return await Task.FromResult<DiscountRule?>(null);
    }
}

public class MockShippingCalculator : IShippingCalculator
{
    public decimal CalculateShippingFee(Order order)
    {
        return order.TotalAmount > 1000m ? 0m : 100m;
    }

    public bool IsEligibleForFreeShipping(Order order)
    {
        return order.TotalAmount > 1000m;
    }
}

public class MockLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }
}