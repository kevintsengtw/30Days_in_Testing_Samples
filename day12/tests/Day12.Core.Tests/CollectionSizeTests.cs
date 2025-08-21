using Day12.Core.Tests.Attributes;

namespace Day12.Core.Tests;

/// <summary>
/// 控制物件集合產生數量的測試
/// </summary>
public class CollectionSizeTests
{
    [Theory]
    [AutoData]
    public void CollectionSize_控制自動產生集合大小(
        [CollectionSize(5)] List<Product> products,
        [CollectionSize(3)] List<Order> orders,
        Customer customer)
    {
        // Arrange & Act - 集合已根據 CollectionSize 產生

        // Assert
        products.Should().HaveCount(5);
        orders.Should().HaveCount(3);
        customer.Should().NotBeNull();

        // 驗證每個 Product 都有合理的值
        products.Should().AllSatisfy(product =>
        {
            product.Name.Should().NotBeNullOrEmpty();
            product.Price.Should().BeGreaterThanOrEqualTo(0);
        });

        // 驗證每個 Order 都有合理的值
        orders.Should().AllSatisfy(order =>
        {
            order.OrderNumber.Should().NotBeNullOrEmpty();
            order.Amount.Should().BeGreaterThanOrEqualTo(0);
        });
    }

    [Theory]
    [AutoData]
    public void CollectionSize_不同大小的集合測試(
        [CollectionSize(2)] List<Customer> customers,
        [CollectionSize(4)] List<Product> products,
        [CollectionSize(1)] List<Order> orders)
    {
        // Arrange & Act - 集合已根據 CollectionSize 產生

        // Assert
        customers.Should().HaveCount(2);
        products.Should().HaveCount(4);
        orders.Should().HaveCount(1);

        // 驗證客戶集合
        customers.Should().AllSatisfy(customer =>
        {
            customer.Person.Should().NotBeNull();
            customer.Person.Name.Should().NotBeNullOrEmpty();
            customer.CreditLimit.Should().BeGreaterThanOrEqualTo(0);
        });

        // 驗證產品集合
        products.Should().AllSatisfy(product =>
        {
            product.Name.Should().NotBeNullOrEmpty();
            product.Description.Should().NotBeNullOrEmpty();
        });

        // 驗證訂單集合
        orders.Should().AllSatisfy(order =>
        {
            order.Status.Should().BeDefined();
            order.OrderNumber.Should().NotBeNullOrEmpty();
        });
    }

    [Theory]
    [AutoData]
    public void CollectionSize_空集合測試(
        [CollectionSize(0)] List<Product> emptyProducts,
        [CollectionSize(3)] List<Customer> customers)
    {
        // Arrange & Act - 集合已根據 CollectionSize 產生

        // Assert
        emptyProducts.Should().BeEmpty();
        customers.Should().HaveCount(3);

        // 確保非空集合的內容正確
        customers.Should().AllSatisfy(customer =>
        {
            customer.Person.Should().NotBeNull();
            customer.Type.Should().NotBeNullOrEmpty();
        });
    }
}