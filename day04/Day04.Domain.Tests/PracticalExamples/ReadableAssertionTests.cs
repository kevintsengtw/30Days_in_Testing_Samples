using Day04.Domain.Models;
using Day04.Domain.Services;

namespace Day04.Domain.Tests.PracticalExamples;

/// <summary>
/// class ReadableAssertionTests - 可讀性斷言測試
/// </summary>
public class ReadableAssertionTests
{
    private readonly OrderService _orderService = new();

    [Fact]
    public void ProcessOrder_有效訂單項目_應計算正確的總金額()
    {
        // Arrange
        var sampleItems = new List<OrderItem>
        {
            new() { ProductId = 1, ProductName = "Laptop", Quantity = 1, Price = 1000m },
            new() { ProductId = 2, ProductName = "Mouse", Quantity = 2, Price = 25m },
            new() { ProductId = 3, ProductName = "Keyboard", Quantity = 1, Price = 75m }
        };

        const decimal expectedTotal = 1000m + (2 * 25m) + 75m; // 1125m

        // Act
        var order = this._orderService.ProcessOrder(sampleItems);

        // Assert

        // 推薦：鏈式斷言，提高可讀性
        order.Should().NotBeNull("因為訂單處理不應該返回空值")
             .And.BeOfType<Order>("因為結果應該是一個有效的訂單物件");

        // 分組相關的斷言
        order.Items.Should().HaveCount(3, "因為我們提供了3個項目")
             .And.AllSatisfy(item =>
             {
                 item.Price.Should().BeGreaterThan(0, "因為所有項目的價格都必須為正數");
                 item.Quantity.Should().BeGreaterThan(0, "因為數量必須為正數");
             });

        // 明確的期望值斷言
        order.TotalAmount.Should().Be(expectedTotal, "因為總金額應為所有項目價格乘以數量的總和");
    }

    [Fact]
    public void ProcessOrder_無效訂單項目_應提供詳細的錯誤訊息()
    {
        // Arrange
        var invalidItems = new List<OrderItem>
        {
            new() { ProductId = 1, ProductName = "Invalid Item", Quantity = 1, Price = -10m }
        };

        // Act
        Action action = () => this._orderService.ProcessOrder(invalidItems);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage("*價格必須為正數*", "因為不允許有負的價格");
    }
}