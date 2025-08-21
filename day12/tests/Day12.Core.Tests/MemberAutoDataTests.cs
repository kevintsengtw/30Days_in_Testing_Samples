namespace Day12.Core.Tests;

/// <summary>
/// MemberAutoData 結合外部資料來源測試
/// </summary>
public class MemberAutoDataTests
{
    public static IEnumerable<object[]> GetProductCategories()
    {
        yield return ["3C產品", "TECH"];
        yield return ["服飾配件", "FASHION"];
        yield return ["居家生活", "HOME"];
        yield return ["運動健身", "SPORTS"];
    }

    public static IEnumerable<object[]> StatusTransitions =>
    [
        [OrderStatus.Created, OrderStatus.Confirmed],
        [OrderStatus.Confirmed, OrderStatus.Shipped],
        [OrderStatus.Shipped, OrderStatus.Delivered],
        [OrderStatus.Delivered, OrderStatus.Completed]
    ];

    [Theory]
    [MemberAutoData(nameof(GetProductCategories))]
    public void MemberAutoData_使用靜態方法資料(
        string categoryName,
        string categoryCode,
        Product product)
    {
        // Arrange
        product.Name = $"[{categoryCode}] {product.Name}";

        // Act
        var categorizedProduct = new CategorizedProduct
        {
            Product = product,
            CategoryName = categoryName,
            CategoryCode = categoryCode
        };

        // Assert
        categorizedProduct.Product.Should().NotBeNull();
        categorizedProduct.CategoryName.Should().Be(categoryName);
        categorizedProduct.CategoryCode.Should().Be(categoryCode);
        categorizedProduct.Product.Name.Should().StartWith($"[{categoryCode}]");
    }

    [Theory]
    [MemberAutoData(nameof(StatusTransitions))]
    public void MemberAutoData_使用屬性資料(
        OrderStatus fromStatus,
        OrderStatus toStatus,
        Order order)
    {
        // Arrange
        order.Status = fromStatus;

        // Act
        var canTransition = order.CanTransitionTo(toStatus);

        // Assert
        canTransition.Should().BeTrue();
        order.Status.Should().Be(fromStatus);
    }
}