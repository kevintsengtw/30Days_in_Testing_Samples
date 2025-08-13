using Day04.Domain.Models;
using Day04.Domain.Services;

namespace Day04.Domain.Tests.BasicAssertions;

/// <summary>
/// class ComplexObjectComparisonTests - 複雜物件比較測試
/// </summary>
public class ComplexObjectComparisonTests
{
    private readonly OrderService _orderService = new();

    [Fact]
    public void BeEquivalentTo_複雜物件完整比較_應匹配所有嵌套屬性()
    {
        var expectedOrder = new Order
        {
            Id = 1,
            CustomerName = "John Doe",
            OrderDate = new DateTime(2024, 1, 15),
            Items =
            [
                new OrderItem { ProductId = 1, ProductName = "Laptop", Quantity = 1, Price = 999.99m },
                new OrderItem { ProductId = 2, ProductName = "Mouse", Quantity = 2, Price = 25.50m }
            ],
            ShippingAddress = new Address
            {
                Street = "123 Main St",
                City = "Anytown",
                ZipCode = "12345"
            }
        };

        var actualOrder = new Order
        {
            Id = 1,
            CustomerName = "John Doe",
            OrderDate = new DateTime(2024, 1, 15),
            Items =
            [
                new OrderItem { ProductId = 1, ProductName = "Laptop", Quantity = 1, Price = 999.99m },
                new OrderItem { ProductId = 2, ProductName = "Mouse", Quantity = 2, Price = 25.50m }
            ],
            ShippingAddress = new Address
            {
                Street = "123 Main St",
                City = "Anytown",
                ZipCode = "12345"
            }
        };

        // 完整深度物件比較
        actualOrder.Should().BeEquivalentTo(expectedOrder);
    }

    [Fact]
    public void BeEquivalentTo_部分屬性比較_應只比較指定欄位()
    {
        var items = new List<OrderItem>
        {
            new() { ProductId = 1, ProductName = "Test Product", Quantity = 1, Price = 100m }
        };

        var order = this._orderService.CreateOrder(items);

        // 部分屬性比較
        order.Should().BeEquivalentTo(new
                                      {
                                          Items = items,
                                          TotalAmount = 100m
                                      },
                                      options => options.ExcludingMissingMembers());
    }
}