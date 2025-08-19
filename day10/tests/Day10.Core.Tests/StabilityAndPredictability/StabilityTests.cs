using Day10.Core.Enums;

namespace Day10.Core.Tests.StabilityAndPredictability;

/// <summary>
/// 穩定性與可預測性測試
/// </summary>
public class StabilityTests : AutoFixtureTestBase
{
    [Fact]
    public void ProcessOrder_穩定性測試_應每次都產生相同結果()
    {
        // Arrange
        var fixture = this.CreateFixture();
        fixture.RepeatCount = 3; // 固定集合大小

        var order = fixture.Build<Order>()
                           .With(x => x.Status, OrderStatus.Completed)
                           .Without(x => x.Customer) // 避免循環參考
                           .Create();

        var processor = new OrderProcessor();

        // Act - 無論執行多少次，相同輸入應該產生相同輸出
        var actual1 = processor.Process(order);
        var actual2 = processor.Process(order);

        // Assert
        actual1.TotalAmount.Should().Be(actual2.TotalAmount);
        actual1.Success.Should().Be(actual2.Success);
    }

    [Fact]
    public void CalculateDiscount_邊界值測試_應處理所有情況()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // 策略：明確設定關鍵值，其他值保持隨機
        var customer = fixture.Build<Customer>()
                              .With(x => x.TotalSpent, 10000m) // 固定關鍵值
                              .Create();                       // 其他屬性保持隨機

        var calculator = new DiscountCalculator();

        // Act
        var discount = calculator.Calculate(customer);

        // Assert
        // 測試邏輯：消費滿 10000 應該有折扣
        Assert.True(discount >= 0.15m);
    }

    [Fact]
    public void GoodTest_明確設定關鍵值_確保測試穩定()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var customer = fixture.Build<Customer>()
                              .With(x => x.Age, 25) // 明確設定年齡
                              .Create();

        var validator = new CustomerValidator();

        // Act
        var isValid = validator.IsAdult(customer);

        // Assert
        isValid.Should().BeTrue(); // 穩定的結果
    }

    [Fact]
    public void CustomBoundaryHandling_自訂範圍_應在指定範圍內()
    {
        // Arrange
        var fixture = new Fixture();

        // 方法 1：使用 Random.Shared.Next() - 最簡潔
        fixture.Customize<User>(c => c.With(x => x.Age, () => Random.Shared.Next(18, 99))); // 18-98 歲

        // Act
        var people = fixture.CreateMany<User>(50);

        // Assert
        people.All(p => p.Age is >= 18 and <= 98).Should().BeTrue();
    }

    [Fact]
    public void SendEmail_正常情況_應成功發送()
    {
        // Arrange
        var fixture = new Fixture();

        // 確保產生有效的電子郵件格式
        var email = fixture.Create<MailAddress>().Address;
        var subject = fixture.Create<string>();
        var body = fixture.Create<string>();

        var service = new EmailService();

        // Act
        var actual = service.SendEmail(email, subject, body);

        // Assert
        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void SendEmail_無效電子郵件_應回傳失敗(string? invalidEmail)
    {
        // Arrange
        var fixture = new Fixture();
        var subject = fixture.Create<string>();
        var body = fixture.Create<string>();
        var service = new EmailService();

        // Act
        var actual = service.SendEmail(invalidEmail!, subject, body);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void PricingCalculation_各種產品_應計算正確()
    {
        // Arrange
        var fixture = new Fixture();
        var calculator = new PriceCalculator();

        // 測試多種產品組合
        for (var i = 0; i < 100; i++)
        {
            var product = fixture.Build<Product>()
                                 .With(x => x.Price, (decimal)(Random.Shared.NextDouble() * 1000))
                                 .Create();

            var quantity = Random.Shared.Next(1, 10);
            var expectedTotal = product.Price * quantity;

            // Act
            var total = calculator.Calculate(product, quantity);

            // Assert
            total.Should().Be(expectedTotal);
        }
    }
}