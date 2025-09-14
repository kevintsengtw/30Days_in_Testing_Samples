/// <summary>
/// 效能問題測試案例
/// 展示執行時間過長的測試問題
/// </summary>
[Trait("Category", "Problematic")]
public class SlowTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public SlowTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _userRepository = Substitute.For<IUserRepository>();
    }

    [Fact]
    public void ProcessOrder_耗時操作_執行時間過長()
    {
        // Arrange
        var user = new User { Id = "user-1", IsActive = true };
        var items = new List<OrderItem>
        {
            new() { ProductId = "prod-1", ProductName = "商品1", Price = 100, Quantity = 2 }
        };

        _userRepository.GetUserByIdAsync("user-1").Returns(user);
        var orderService = new OrderService(_orderRepository, _userRepository);

        // Act
        // 這個測試會因為 OrderService 內的 Task.Delay 而執行緩慢
        var result = orderService.ProcessOrderAsync("user-1", items).Result;

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user-1");
        result.TotalAmount.Should().Be(200);
    }

    [Fact]
    public void CalculateOrderTotal_多次查詢_累積延遲()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new() { ProductId = "prod-1", ProductName = "商品1", Price = 100, Quantity = 1 }
        };

        var orderService = new OrderService(_orderRepository, _userRepository);

        // Act
        var results = new List<decimal>();

        // 多次呼叫會累積延遲，讓測試變得非常慢
        for (int i = 0; i < 5; i++)
        {
            var total = orderService.CalculateOrderTotalAsync(items, "SAVE10").Result;
            results.Add(total);
        }

        // Assert
        results.All(r => r == 90).Should().BeTrue(); // 100 * 0.9 = 90
    }

    [Fact]
    public void ProcessLargeOrderBatch_記憶體密集_可能造成記憶體問題()
    {
        // Arrange
        var user = new User { Id = "user-1", IsActive = true };
        _userRepository.GetUserByIdAsync("user-1").Returns(user);

        var orderService = new OrderService(_orderRepository, _userRepository);
        var allOrders = new List<Order>();

        // Act
        // 建立大量訂單，可能造成記憶體壓力（減少數量以縮短測試時間）
        for (int i = 0; i < 50; i++)
        {
            var items = new List<OrderItem>
            {
                new() { ProductId = $"prod-{i}", ProductName = $"商品{i}", Price = 100, Quantity = 1 }
            };

            var order = orderService.ProcessOrderAsync("user-1", items).Result;
            allOrders.Add(order);
        }

        // Assert
        allOrders.Count.Should().Be(50);
        allOrders.All(o => o.TotalAmount == 100).Should().BeTrue();
    }

    [Fact]
    public void ComplexCalculation_CPU密集_長時間運算()
    {
        // Arrange
        var orderService = new OrderService(_orderRepository, _userRepository);

        // Act
        var results = new List<decimal>();

        // 模擬複雜的計算邏輯（減少執行次數）
        for (int i = 0; i < 10; i++)
        {
            var items = new List<OrderItem>();

            // 建立大量訂單項目進行計算
            for (int j = 0; j < 20; j++)
            {
                items.Add(new OrderItem
                {
                    ProductId = $"prod-{j}",
                    ProductName = $"商品{j}",
                    Price = j + 1,
                    Quantity = (j % 5) + 1
                });
            }

            var total = orderService.CalculateOrderTotalAsync(items).Result;
            results.Add(total);
        }

        // Assert
        results.Count.Should().Be(10);
        results.All(r => r > 0).Should().BeTrue();
    }
}
