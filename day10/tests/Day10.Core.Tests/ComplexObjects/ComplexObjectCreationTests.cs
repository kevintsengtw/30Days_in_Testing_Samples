using Day10.Core.Enums;

namespace Day10.Core.Tests.ComplexObjects;

/// <summary>
/// 複雜物件建構測試
/// </summary>
public class ComplexObjectCreationTests : AutoFixtureTestBase
{
    [Fact]
    public void AutoFixture_巢狀物件_應完整建構所有層級()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // Act
        var customer = fixture.Create<Customer>();

        // Assert
        customer.Should().NotBeNull();
        customer.Id.Should().BePositive();
        customer.Name.Should().NotBeNullOrEmpty();
        customer.Email.Should().NotBeNullOrEmpty();

        // 巢狀物件應該被建立
        customer.Address.Should().NotBeNull();
        customer.Address!.Street.Should().NotBeNullOrEmpty();
        customer.Address.Location.Should().NotBeNull();
        customer.Address.Location!.Latitude.Should().NotBe(0);

        customer.ContactInfo.Should().NotBeNull();
        customer.ContactInfo!.Phone.Should().NotBeNullOrEmpty();

        // 列舉應該有有效值
        customer.Type.Should().BeOneOf(CustomerType.Regular, CustomerType.Premium, CustomerType.VIP);
    }

    [Fact]
    public void AutoFixture_集合處理_應建立包含元素的集合()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // Act
        var order = fixture.Build<Order>()
                           .Without(x => x.Customer) // 避免循環參考
                           .Create();

        // Assert
        order.Items.Should().NotBeNull();
        order.Items.Should().NotBeEmpty();
        order.Items.Should().HaveCountGreaterThan(0);

        order.Tags.Should().NotBeNull();
        order.Tags.Should().NotBeEmpty();

        order.Metadata.Should().NotBeNull();
        order.Metadata.Should().NotBeEmpty();

        order.CategoryIds.Should().NotBeNull();
        order.CategoryIds.Should().NotBeEmpty();

        // 驗證集合元素的內容
        var firstItem = order.Items.First();
        firstItem.ProductId.Should().BePositive();
        firstItem.ProductName.Should().NotBeNullOrEmpty();
        firstItem.Quantity.Should().BePositive();
        firstItem.UnitPrice.Should().BePositive();
    }

    [Fact]
    public void AutoFixture_邊界值處理_應避免常見問題()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // Act
        // AutoFixture 預設避免常見問題值
        var strings = fixture.CreateMany<string>(10); // 減少數量以提高測試速度

        // 所有字串都不會是 null 或空字串
        strings.All(s => !string.IsNullOrEmpty(s)).Should().BeTrue();

        var numbers = fixture.CreateMany<int>(10); // 減少數量

        // 數字預設是正數
        numbers.All(n => n > 0).Should().BeTrue();
    }

    [Fact]
    public void AutoFixture_循環參考_應正常處理不會無限遞迴()
    {
        // Arrange
        var fixture = this.CreateFixture(); // 使用已配置的 fixture

        // Act & Assert
        var category = fixture.Create<Category>();

        category.Should().NotBeNull();
        category.Id.Should().BePositive();
        category.Name.Should().NotBeNullOrEmpty();

        // 注意：因為使用 OmitOnRecursionBehavior，循環參考的屬性可能被設為 null
        // 所以這裡的驗證要相應調整
    }

    [Fact]
    public void AutoFixture_預設循環處理_應拋出例外()
    {
        // Arrange
        var defaultFixture = this.CreateDefaultFixture();

        // Act & Assert
        // 使用 Should().Throw() 驗證預期的例外
        Action act = () => defaultFixture.Create<Category>();
        act.Should().Throw<ObjectCreationException>()
           .WithMessage("*recursion*"); // 驗證例外訊息包含遞迴相關內容
    }

    [Fact]
    public void AutoFixture_OmitOnRecursion_應成功建立物件()
    {
        // Arrange
        var fixture = new Fixture();

        // 移除預設的 ThrowingRecursionBehavior(會拋出例外)
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
               .ForEach(b => fixture.Behaviors.Remove(b));

        // 加入 OmitOnRecursionBehavior(遇到循環參考時設為 null)
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Act
        var category = fixture.Create<Category>();

        // Assert
        category.Should().NotBeNull();

        // 遞迴屬性在達到深度限制時會被設為 null
        // 這樣可以避免無限遞迴，同時保持物件的基本結構
    }
}