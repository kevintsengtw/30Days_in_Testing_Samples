using Day12.Core.Tests.Attributes;

namespace Day12.Core.Tests;

/// <summary>
/// CompositeAutoData 多重資料來源整合測試
/// </summary>
public class CompositeAutoDataTests
{
    [Theory]
    [CompositeAutoData(typeof(DomainAutoDataAttribute), typeof(BusinessAutoDataAttribute))]
    public void CompositeAutoData_整合多重資料來源(
        Person person,
        Product product,
        Order order)
    {
        // Arrange
        // 所有物件都已經根據各自的 AutoData 配置產生

        // Act
        var orderItem = new OrderItem
        {
            ProductId = Random.Shared.Next(1, 1000),
            Product = product,
            Quantity = 2
        };

        // Assert
        // 驗證 DomainAutoData 的設定
        person.Age.Should().BeInRange(18, 64);
        person.Email.Should().EndWith("@example.com");
        person.Name.Should().StartWith("測試用戶");

        product.Price.Should().BeInRange(100, 9999);
        product.IsAvailable.Should().BeTrue();
        product.Name.Should().StartWith("產品");

        // 驗證 BusinessAutoData 的設定
        order.Status.Should().Be(OrderStatus.Created);
        order.Amount.Should().BeInRange(1000, 49999);
        order.OrderNumber.Should().StartWith("ORD");

        orderItem.Should().NotBeNull();
        orderItem.ProductId.Should().BePositive();
        orderItem.Quantity.Should().Be(2);
    }

    [Theory]
    [DomainAutoData]
    public void DomainAutoData_領域物件自訂配置(Person person, Product product)
    {
        // Arrange & Act - 參數由 DomainAutoData 產生

        // Assert
        person.Name.Should().StartWith("測試用戶");
        person.Age.Should().BeInRange(18, 64);
        person.Email.Should().EndWith("@example.com");

        product.Name.Should().StartWith("產品");
        product.Price.Should().BeInRange(100, 9999);
        product.IsAvailable.Should().BeTrue();
    }

    [Theory]
    [BusinessAutoData]
    public void BusinessAutoData_業務邏輯自訂配置(Order order)
    {
        // Arrange & Act - 參數由 BusinessAutoData 產生

        // Assert
        order.Status.Should().Be(OrderStatus.Created);
        order.Amount.Should().BeInRange(1000, 49999);
        order.OrderNumber.Should().StartWith("ORD");
        order.OrderNumber.Length.Should().Be(15); // ORD + 8位日期 + 4位隨機數
    }
}