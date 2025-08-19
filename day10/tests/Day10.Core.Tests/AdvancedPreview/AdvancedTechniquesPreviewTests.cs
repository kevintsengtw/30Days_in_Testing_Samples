using Day10.Core.Enums;

namespace Day10.Core.Tests.AdvancedPreview;

/// <summary>
/// 進階技巧預覽 (明日內容預告)
/// </summary>
public class AdvancedTechniquesPreviewTests : AutoFixtureTestBase
{
    [Fact]
    public void CustomizationPreview_自訂特定屬性()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 自訂特定屬性
        fixture.Customize<Customer>(c => c.With(x => x.Email, "test@example.com")
                                          .Without(x => x.ContactInfo)); // 排除聯絡資訊

        // Act
        var customer = fixture.Create<Customer>();

        // Assert
        customer.Email.Should().Be("test@example.com");
        customer.ContactInfo.Should().BeNull();
        customer.Name.Should().NotBeNullOrEmpty(); // 其他屬性仍自動產生
    }

    [Fact]
    public void ConditionalCustomization_條件式客製化()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 條件式客製化
        fixture.Customize<Order>(c => c.With(x => x.Status, OrderStatus.Pending)
                                       .With(x => x.OrderDate, DateTime.Today));

        // Act
        var order = fixture.Create<Order>();

        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
        order.OrderDate.Date.Should().Be(DateTime.Today);
    }

    [Fact]
    public void FactoryMethodCustomization_工廠方法客製化()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 使用 Build 方法建立物件，確保屬性設定生效
        fixture.Customize<Product>(c => c.With(x => x.Price, 999.99m)
                                         .With(x => x.Category, "Special"));

        // Act
        var product = fixture.Create<Product>();

        // Assert
        product.Name.Should().NotBeNullOrEmpty();
        product.Price.Should().Be(999.99m);
        product.Category.Should().Be("Special");
    }

    /// <summary>
    /// 自訂的 Customization 類別
    /// </summary>
    public class BusinessRuleCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            // VIP 客戶的商業規則
            fixture.Customize<Customer>(c => c.With(x => x.Type, CustomerType.VIP)
                                              .With(x => x.TotalSpent, () => Random.Shared.Next(50000, 200000))
                                              .With(x => x.Age, () => Random.Shared.Next(25, 65)));
        }
    }

    [Fact]
    public void BusinessRuleCustomization_商業規則客製化()
    {
        // Arrange
        var fixture = this.CreateFixture();
        fixture.Customize(new BusinessRuleCustomization());

        // Act
        var customer = fixture.Create<Customer>();

        // Assert
        customer.Type.Should().Be(CustomerType.VIP);
        customer.TotalSpent.Should().BeInRange(50000, 199999);
        customer.Age.Should().BeInRange(25, 64);
    }

    [Fact]
    public void BuilderPatternWithAutoFixture_結合建造者模式()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 結合 AutoFixture 和 Builder Pattern
        var baseCustomer = fixture.Create<Customer>();

        var vipCustomer = fixture.Build<Customer>()
                                 .With(x => x.Id, baseCustomer.Id)     // 使用基礎客戶的 ID
                                 .With(x => x.Name, baseCustomer.Name) // 使用基礎客戶的名稱
                                 .With(x => x.Type, CustomerType.VIP)
                                 .With(x => x.TotalSpent, 100000m)
                                 .Create();

        // Act & Assert
        vipCustomer.Type.Should().Be(CustomerType.VIP);
        vipCustomer.TotalSpent.Should().Be(100000m);
        vipCustomer.Name.Should().Be(baseCustomer.Name); // 保持原有名稱
    }

    [Fact]
    public void MultipleCustomizations_多重客製化()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 多重客製化 - 合併到單一客製化中
        fixture.Customize<Customer>(c => c.With(x => x.Age, 30)
                                          .With(x => x.Type, CustomerType.Premium));

        fixture.Customize<Order>(c => c.With(x => x.Status, OrderStatus.Completed));

        // Act
        var customer = fixture.Create<Customer>();
        var order = fixture.Create<Order>();

        // Assert
        customer.Age.Should().Be(30);
        customer.Type.Should().Be(CustomerType.Premium);
        order.Status.Should().Be(OrderStatus.Completed);
    }
}