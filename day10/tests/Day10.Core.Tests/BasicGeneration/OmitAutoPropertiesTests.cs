using Day10.Core.Enums;

namespace Day10.Core.Tests.BasicGeneration;

/// <summary>
/// OmitAutoProperties 使用示範測試
/// </summary>
public class OmitAutoPropertiesTests : AutoFixtureTestBase
{
    [Fact]
    public void CreateCustomer_僅設定必要屬性_其他保持預設值()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 使用 OmitAutoProperties 控制屬性設定
        var customer = fixture.Build<Customer>()
                              .OmitAutoProperties() // 不自動設定任何屬性
                              .With(x => x.Id, 123) // 只設定我們關心的屬性
                              .With(x => x.Name, "測試客戶")
                              .Create();

        // Act & Assert
        customer.Id.Should().Be(123);
        customer.Name.Should().Be("測試客戶");

        // 其他屬性保持預設值
        customer.Email.Should().Be(string.Empty);
        customer.Age.Should().Be(0);
        customer.Type.Should().Be(CustomerType.Regular); // 列舉的預設值
    }

    [Fact]
    public void CreateCustomer_部分自動屬性_部分手動設定()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 結合 OmitAutoProperties 和選擇性屬性設定
        var customer = fixture.Build<Customer>()
                              .OmitAutoProperties()                   // 先停用所有自動屬性
                              .With(x => x.Id)                        // 啟用 Id 的自動產生
                              .With(x => x.Name)                      // 啟用 Name 的自動產生
                              .With(x => x.Email, "test@example.com") // 手動設定 Email
                              .Create();

        // Act & Assert
        customer.Id.Should().NotBe(0);                  // 自動產生的值
        customer.Name.Should().NotBeNullOrEmpty();      // 自動產生的值
        customer.Email.Should().Be("test@example.com"); // 手動設定的值

        // 未指定的屬性保持預設值
        customer.Age.Should().Be(0);
        customer.Type.Should().Be(CustomerType.Regular);
    }

    [Fact]
    public void CreateProduct_測試特定屬性_忽略其他屬性()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 場景：我們只關心產品的名稱和價格，其他屬性不重要
        var product = fixture.Build<Product>()
                             .OmitAutoProperties()
                             .With(x => x.Name, "特殊產品")
                             .With(x => x.Price, 999.99m)
                             .Create();

        // Act & Assert
        // 只驗證我們關心的屬性
        product.Name.Should().Be("特殊產品");
        product.Price.Should().Be(999.99m);

        // 其他屬性保持預設值，不會影響測試
        product.Id.Should().Be(0);
        product.Category.Should().Be(string.Empty);
        product.InStock.Should().BeFalse();
    }

    [Fact]
    public void CreateOrder_避免不必要的屬性設定_提高效能()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 場景：建立大量簡單訂單物件，只需要基本資訊
        var orders = fixture.Build<Order>()
                            .OmitAutoProperties() // 避免設定所有屬性
                            .With(x => x.Id)      // 只設定必要的屬性
                            .With(x => x.OrderDate, DateTime.Today)
                            .With(x => x.Status, OrderStatus.Pending)
                            .CreateMany(100) // 建立 100 個訂單
                            .ToList();

        // Act & Assert
        orders.Should().HaveCount(100);
        orders.Should().OnlyContain(o => o.OrderDate == DateTime.Today);
        orders.Should().OnlyContain(o => o.Status == OrderStatus.Pending);
        orders.Should().OnlyContain(o => o.Id != 0);

        // 未設定的屬性保持預設值
        orders.Should().OnlyContain(o => o.Customer == null);
    }

    [Fact]
    public void CreateOrderItem_驗證預設值行為()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 測試物件的預設狀態是否正確
        var orderItem = fixture.Build<OrderItem>()
                               .OmitAutoProperties() // 完全不設定任何屬性
                               .Create();

        // Act & Assert
        // 驗證所有屬性都是預設值
        orderItem.ProductId.Should().Be(0);
        orderItem.Quantity.Should().Be(0);
        orderItem.UnitPrice.Should().Be(0);
        orderItem.Product.Should().BeNull();
    }

    [Fact]
    public void CreateAddress_比較OmitAutoProperties與完全自動產生()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // Act
        // 使用 OmitAutoProperties 的版本
        var addressWithOmit = fixture.Build<Address>()
                                     .OmitAutoProperties()
                                     .With(x => x.Street, "台北市信義區")
                                     .With(x => x.City, "台北市")
                                     .Create();

        // 完全自動產生的版本
        var addressAuto = fixture.Create<Address>();

        // Assert
        // OmitAutoProperties 版本只有指定的屬性有值
        addressWithOmit.Street.Should().Be("台北市信義區");
        addressWithOmit.City.Should().Be("台北市");
        addressWithOmit.PostalCode.Should().Be(string.Empty); // 預設值
        addressWithOmit.Country.Should().Be(string.Empty);    // 預設值

        // 自動產生版本所有屬性都有值
        addressAuto.Street.Should().NotBeNullOrEmpty();
        addressAuto.City.Should().NotBeNullOrEmpty();
        addressAuto.PostalCode.Should().NotBeNullOrEmpty();
        addressAuto.Country.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(CustomerType.Regular)]
    [InlineData(CustomerType.Premium)]
    [InlineData(CustomerType.VIP)]
    public void CreateCustomer_設定特定類型_其他屬性使用預設值(CustomerType customerType)
    {
        // Arrange
        var fixture = this.CreateFixture();

        // Act
        var customer = fixture.Build<Customer>()
                              .OmitAutoProperties()
                              .With(x => x.Type, customerType) // 只設定客戶類型
                              .Create();

        // Assert
        customer.Type.Should().Be(customerType);

        // 其他屬性都是預設值
        customer.Id.Should().Be(0);
        customer.Name.Should().Be(string.Empty);
        customer.Email.Should().Be(string.Empty);
        customer.Age.Should().Be(0);
    }
}