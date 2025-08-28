using Day19.WebApplication.Controllers.Examples.Level2;
using NSubstitute;

namespace Day19.WebApplication.Integration.Tests.Examples.Level2;

/// <summary>
/// Level 2 整合測試：Service Stub 測試
/// 測試重點：Controller 與 Service 的整合、錯誤處理、業務邏輯驗證
/// 特色：使用 NSubstitute 模擬服務依賴，專注於整合層邏輯
/// </summary>
public class ServiceDependentControllerTests : IClassFixture<ServiceStubWebApplicationFactory>
{
    private readonly ServiceStubWebApplicationFactory _factory;

    public ServiceDependentControllerTests(ServiceStubWebApplicationFactory factory)
    {
        this._factory = factory;
    }

    [Fact]
    public async Task GetOrder_存在的訂單_應回傳訂單資料()
    {
        // Arrange
        var orderId = 123;
        var expectedOrder = new Order
        {
            Id = orderId,
            ProductName = "Test Product",
            Quantity = 2,
            CustomerEmail = "test@example.com",
            Status = "Pending"
        };

        this._factory.OrderServiceStub.GetOrderAsync(orderId).Returns(expectedOrder);

        using var client = this._factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/v2/servicedependent/orders/{orderId}");

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<Order>(order =>
                {
                    order.Id.Should().Be(orderId);
                    order.ProductName.Should().Be("Test Product");
                    order.Quantity.Should().Be(2);
                    order.CustomerEmail.Should().Be("test@example.com");
                    order.Status.Should().Be("Pending");
                });

        // 驗證服務呼叫
        await this._factory.OrderServiceStub.Received(1).GetOrderAsync(orderId);
    }

    [Fact]
    public async Task GetOrder_不存在的訂單_應回傳404NotFound()
    {
        // Arrange
        var orderId = 999;
        this._factory.OrderServiceStub.GetOrderAsync(orderId).Returns((Order?)null);

        using var client = this._factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/v2/servicedependent/orders/{orderId}");

        // Assert
        response.Should().Be404NotFound();

        // 驗證錯誤訊息（原始 JSON，需要手動解析）
        var content = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<JsonElement>(content);

        errorResponse.GetProperty("error").GetString().Should().Be("Order 999 not found");

        await this._factory.OrderServiceStub.Received(1).GetOrderAsync(orderId);
    }

    [Fact]
    public async Task CreateOrder_有效資料且有庫存_應建立訂單並發送通知()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            ProductName = "Test Product",
            Quantity = 2,
            CustomerEmail = "customer@example.com"
        };

        var createdOrder = new Order
        {
            Id = 456,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            CustomerEmail = request.CustomerEmail,
            Status = "Pending"
        };

        this._factory.InventoryServiceStub.CheckStockAsync(request.ProductName, request.Quantity).Returns(true);
        this._factory.OrderServiceStub.CreateOrderAsync(request.ProductName, request.Quantity, request.CustomerEmail)
            .Returns(createdOrder);

        using var client = this._factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/v2/servicedependent/orders", request);

        // Assert
        response.Should().Be201Created()
                .And
                .Satisfy<Order>(order =>
                {
                    order.Id.Should().Be(456);
                    order.ProductName.Should().Be("Test Product");
                    order.Quantity.Should().Be(2);
                    order.CustomerEmail.Should().Be("customer@example.com");
                    order.Status.Should().Be("Pending");
                });

        // 驗證 Location 標頭（手動檢查，因為 AwesomeAssertions.Web 不支援 HaveLocationHeader）
        response.Headers.Location.Should().NotBeNull();

        // 驗證所有服務都被正確呼叫
        await this._factory.InventoryServiceStub.Received(1).CheckStockAsync(request.ProductName, request.Quantity);
        await this._factory.OrderServiceStub.Received(1).CreateOrderAsync(request.ProductName, request.Quantity, request.CustomerEmail);
        await this._factory.NotificationServiceStub.Received(1).SendOrderConfirmationAsync(456, request.CustomerEmail);
    }
}