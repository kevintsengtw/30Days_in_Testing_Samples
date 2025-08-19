using Day10.Core.Enums;

namespace Day10.Core.Tests.Comparison;

/// <summary>
/// Day 10 AutoFixture 方式的範例
/// </summary>
public class Day10AutoFixtureApproachTests : AutoFixtureTestBase
{
    [Fact]
    public void ProcessOrder_AutoFixture方式_專注於測試邏輯()
    {
        // Arrange - 大幅簡化，只設定測試關心的部分
        var fixture = this.CreateFixture();
        var order = fixture.Build<Order>()
                           .With(x => x.Status, OrderStatus.Completed) // 只設定測試關心的狀態
                           .Create();                                  // 其他屬性自動產生合理值

        var processor = new OrderProcessor();

        // Act
        var actual = processor.Process(order);

        // Assert
        actual.Success.Should().BeTrue();
        actual.TotalAmount.Should().BePositive();
    }

    [Fact]
    public void ProcessOrder_大量訂單測試_AutoFixture展現威力()
    {
        // Arrange - 輕鬆建立 100 筆測試資料
        var fixture = this.CreateFixture();
        var orders = fixture.Build<Order>()
                            .With(x => x.Status, OrderStatus.Completed)
                            .CreateMany(100)
                            .ToList();

        var processor = new BatchOrderProcessor();

        // Act
        var actual = processor.ProcessBatch(orders);

        // Assert
        actual.SuccessCount.Should().Be(100);
        actual.FailureCount.Should().Be(0);
    }

    [Theory]
    [InlineData(true)]  // 期望驗證通過
    [InlineData(false)] // 期望驗證失敗
    public void ValidateUser_AutoFixture方式_動態產生資料(bool shouldBeValid)
    {
        // Arrange
        var fixture = new Fixture();
        var user = fixture.Build<User>()
                          .With(x => x.Name, shouldBeValid ? fixture.Create<string>() : "")
                          .With(x => x.Email, shouldBeValid ? fixture.Create<MailAddress>().Address : "invalid-email")
                          .With(x => x.Age, shouldBeValid ? 25 : 10)
                          .Create();
        var validator = new UserValidator();

        // Act
        var actual = validator.IsValid(user);

        // Assert
        actual.Should().Be(shouldBeValid);
    }

    [Fact]
    public void ValidateUser_AutoFixture大量情境_自動涵蓋各種案例()
    {
        var fixture = new Fixture();

        // 測試 100 個有效使用者(隨機產生但符合規則)
        for (var i = 0; i < 100; i++)
        {
            // Arrange
            var validUser = fixture.Build<User>()
                                   .With(x => x.Email, fixture.Create<MailAddress>().Address)
                                   .With(x => x.Age, Random.Shared.Next(18, 99)) // 18-98 歲
                                   .Create();

            var validator = new UserValidator();

            // Act
            var actual = validator.IsValid(validUser);

            // Assert
            actual.Should().BeTrue($"Generated user should be valid: {validUser.Name}");
        }
    }
}