using Day10.Core.Enums;

namespace Day10.Core.Tests.XunitIntegration;

/// <summary>
/// AutoFixture 與 xUnit 整合測試
/// </summary>
public class XunitIntegrationTests : AutoFixtureTestBase
{
    [Fact]
    public void CreateProduct_有效資料_應成功建立產品()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var productData = fixture.Create<ProductCreateRequest>();
        var service = new ProductService();

        // Act
        var actual = service.CreateProduct(productData);

        // Assert
        actual.Should().NotBeNull();
        actual.Id.Should().BePositive();
    }

    [Fact]
    public void UpdateProduct_有效資料_應成功更新產品()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var updateData = fixture.Create<ProductUpdateRequest>();
        var service = new ProductService();

        // Act
        var actual = service.UpdateProduct(1, updateData);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void ProcessOrder_正常流程_應成功處理()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 快速建立測試資料，避免循環參考
        var customer = fixture.Create<Customer>();
        var products = fixture.CreateMany<Product>(3).ToList();
        var order = fixture.Build<Order>()
                           .With(x => x.Customer, customer)
                           .Without(x => x.Customer) // 避免循環參考
                           .With(x => x.Status, OrderStatus.Completed)
                           .With(x => x.Items, products.Select(p => new OrderItem
                           {
                               Product = p,
                               Quantity = 2 // 使用固定數量，避免隨機性影響測試
                           }).ToList())
                           .Create();

        var processor = new OrderProcessor();

        // Act
        var actual = processor.Process(order);

        // Assert
        actual.Success.Should().BeTrue(); // 因為狀態設為 Completed
    }

    [Fact]
    public void BulkProcessOrders_多筆訂單_應全部處理成功()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var orders = fixture.Build<Order>()
                            .With(x => x.Status, OrderStatus.Completed)
                            .Without(x => x.Customer) // 避免循環參考
                            .CreateMany(5)
                            .ToList(); // 固定 5 筆，更可預測

        var processor = new BatchOrderProcessor();

        // Act
        var actual = processor.ProcessBatch(orders);

        // Assert
        actual.Should().NotBeNull();
        actual.SuccessCount.Should().Be(5);
        actual.FailureCount.Should().Be(0);
    }

    [Theory]
    [InlineData(CustomerType.Regular)]
    [InlineData(CustomerType.Premium)]
    [InlineData(CustomerType.VIP)]
    public void CalculateDiscount_不同客戶類型_應套用正確折扣(CustomerType customerType)
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 建立客戶，但指定特定的客戶類型
        var customer = fixture.Build<Customer>()
                              .With(x => x.Type, customerType)
                              .Create();

        var order = fixture.Build<Order>()
                           .Without(x => x.Customer) // 避免循環參考
                           .Create();
        var calculator = new DiscountCalculator();

        // Act
        var discount = calculator.Calculate(customer, order);

        // Assert
        switch (customerType)
        {
            case CustomerType.Regular:
                discount.Should().Be(0);
                break;
            case CustomerType.Premium:
                discount.Should().Be(0.05m);
                break;
            case CustomerType.VIP:
                discount.Should().Be(0.15m);
                break;
        }
    }

    [Theory]
    [AutoDataWithCircularReferenceHandling]
    public void AutoData_多個參數_應全部自動生成(
        Customer customer,
        Product product)
    {
        // Assert
        // 所有參數都應該被自動填充
        customer.Should().NotBeNull();
        customer.Id.Should().BePositive();
        customer.Name.Should().NotBeNullOrEmpty();

        product.Should().NotBeNull();
        product.Id.Should().BePositive();
        product.Name.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [AutoDataWithCircularReferenceHandling]
    public void AutoData_集合類型_應自動填充元素(
        List<string> stringList,
        IEnumerable<int> intEnumerable,
        Dictionary<string, object> dictionary)
    {
        // Assert
        stringList.Should().NotBeNull();
        stringList.Should().NotBeEmpty();
        stringList.Should().HaveCountGreaterThan(0);

        intEnumerable.Should().NotBeNull();
        intEnumerable.Should().NotBeEmpty();

        dictionary.Should().NotBeNull();
        dictionary.Should().NotBeEmpty();
    }
}