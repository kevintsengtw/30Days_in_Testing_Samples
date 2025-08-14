using Day05.Domain.DomainModels;
using Day05.Domain.Services.BusinessServices;
using Day05.Tests.Extensions;

namespace Day05.Domain.Tests.CustomAssertionTests;

public class OrderServiceTests
{
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        this._orderService = new OrderService();
    }

    [Fact]
    public void ProcessOrder_有效項目_應回傳有效訂單()
    {
        // Arrange
        var items = new[]
        {
            new OrderItem { ProductId = 1, Quantity = 2, Price = 50.0m },
            new OrderItem { ProductId = 2, Quantity = 1, Price = 100.0m }
        };

        // Act
        var actual = this._orderService.CreateOrder(items);

        // Assert
        // 使用領域特定 Assertions
        actual.Should().BeValidOrder();
        actual.Items.Should().HaveCount(2);
    }
}