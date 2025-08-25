namespace Day16.Core.Tests;

/// <summary>
/// OrderService 的傳統測試範例
/// 此類別展示傳統的測試寫法：手動建立 FakeTimeProvider 和 OrderService 實例
/// 可與 OrderServiceAutoFixtureTests.cs 進行對比，觀察兩種寫法的差異
/// 所有測試方法都使用一致的 [Fact] 或 [Theory] 屬性，不使用 AutoFixture
/// </summary>
public class OrderServiceTests
{
    [Fact]
    public void CanPlaceOrder_在營業時間內_應回傳True()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 14, 0, 0)); // 下午 2 點

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.CanPlaceOrder();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPlaceOrder_在營業時間外_應回傳False()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 20, 0, 0)); // 晚上 8 點

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.CanPlaceOrder();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(8, false)]  // 上午8點 - 營業時間前
    [InlineData(9, true)]   // 上午9點 - 剛開始營業
    [InlineData(12, true)]  // 中午12點 - 營業時間內
    [InlineData(16, true)]  // 下午4點 - 營業時間內
    [InlineData(17, false)] // 下午5點 - 剛結束營業
    [InlineData(18, false)] // 下午6點 - 營業時間後
    public void CanPlaceOrder_不同時間點_應回傳正確結果(int hour, bool expected)
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, hour, 0, 0));

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.CanPlaceOrder();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetTimeBasedDiscount_週五_應回傳週五折扣()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        // 設定為週五
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 10, 0, 0)); // 2024/3/15 是週五

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.GetTimeBasedDiscount();

        // Assert
        result.Should().Be("週五快樂：九折優惠");
    }

    [Fact]
    public void GetTimeBasedDiscount_聖誕節_應回傳聖誕折扣()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 12, 25, 10, 0, 0)); // 聖誕節

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.GetTimeBasedDiscount();

        // Assert
        result.Should().Be("聖誕特惠：八折優惠");
    }

    [Fact]
    public void GetTimeBasedDiscount_一般日期_應回傳無優惠()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 14, 10, 0, 0)); // 2024/3/14 是週四

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.GetTimeBasedDiscount();

        // Assert
        result.Should().Be("無優惠");
    }

    [Fact]
    public void CanPlaceOrder_營業時間開始時_應回傳True()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 9, 0, 0)); // 上午9點整

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.CanPlaceOrder();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanPlaceOrder_營業時間結束時_應回傳False()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 17, 0, 0)); // 下午5點整

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var result = orderService.CanPlaceOrder();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetTimeBasedDiscount_週一_應回傳無優惠()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 11, 14, 0, 0)); // 2024/3/11 是週一

        var orderService = new OrderService(fakeTimeProvider);

        // Act
        var discount = orderService.GetTimeBasedDiscount();

        // Assert
        discount.Should().Be("無優惠");
    }

    [Fact]
    public void Constructor_傳入Null_應拋出ArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new OrderService(null!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("timeProvider");
    }
}