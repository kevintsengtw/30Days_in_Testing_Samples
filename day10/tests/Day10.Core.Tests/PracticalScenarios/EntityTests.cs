using Day10.Core.Enums;

namespace Day10.Core.Tests.PracticalScenarios;

/// <summary>
/// Entity 測試
/// </summary>
public class EntityTests : AutoFixtureTestBase
{
    [Theory]
    [InlineData(0, CustomerLevel.Bronze)]
    [InlineData(5000, CustomerLevel.Bronze)]
    [InlineData(15000, CustomerLevel.Silver)]
    [InlineData(60000, CustomerLevel.Gold)]
    [InlineData(120000, CustomerLevel.Diamond)]
    public void GetLevel_不同消費金額_應回傳正確等級(decimal totalSpent, CustomerLevel expectedLevel)
    {
        // Arrange
        var fixture = this.CreateFixture();
        var customer = fixture.Build<Customer>()
                              .With(x => x.TotalSpent, totalSpent)
                              .Create();

        // Act
        var level = customer.GetLevel();

        // Assert
        level.Should().Be(expectedLevel);
    }

    [Fact]
    public void CanGetDiscount_符合條件_應可獲得折扣()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var customer = fixture.Build<Customer>()
                              .With(x => x.TotalSpent, 5000m)
                              .With(x => x.Orders, fixture.CreateMany<Order>(10).ToList())
                              .Create();

        // Act
        var canDiscount = customer.CanGetDiscount();

        // Assert
        canDiscount.Should().BeTrue();
    }

    [Fact]
    public void CanGetDiscount_不符合消費金額_應無法獲得折扣()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var customer = fixture.Build<Customer>()
                              .With(x => x.TotalSpent, 500m)
                              .With(x => x.Orders, fixture.CreateMany<Order>(10).ToList())
                              .Create();

        // Act
        var canDiscount = customer.CanGetDiscount();

        // Assert
        canDiscount.Should().BeFalse();
    }

    [Fact]
    public void CanGetDiscount_不符合訂單數量_應無法獲得折扣()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var customer = fixture.Build<Customer>()
                              .With(x => x.TotalSpent, 5000m)
                              .With(x => x.Orders, fixture.CreateMany<Order>(3).ToList())
                              .Create();

        // Act
        var canDiscount = customer.CanGetDiscount();

        // Assert
        canDiscount.Should().BeFalse();
    }
}