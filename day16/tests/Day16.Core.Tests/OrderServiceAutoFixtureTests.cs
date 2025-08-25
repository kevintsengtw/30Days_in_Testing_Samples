namespace Day16.Core.Tests;

/// <summary>
/// OrderService 的 AutoFixture 測試範例
/// 此類別展示如何使用 AutoFixture 簡化測試程式碼
/// 可與 OrderServiceTests.cs 進行對比，觀察兩種寫法的差異
/// </summary>
public class OrderServiceAutoFixtureTests
{
    [Theory]
    [AutoDataWithCustomization]
    public void CanPlaceOrder_在營業時間內_應回傳True(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 14, 0, 0)); // 週五下午2點

        // Act
        var result = sut.CanPlaceOrder();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [AutoDataWithCustomization]
    public void CanPlaceOrder_在營業時間外_應回傳False(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 22, 0, 0)); // 週五晚上10點

        // Act
        var result = sut.CanPlaceOrder();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [AutoDataWithCustomization]
    public void CanPlaceOrder_在週末_但營業時間內_應回傳True(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange - 週六下午2點（營業時間內）
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 16, 14, 0, 0)); // 週六下午2點

        // Act
        var result = sut.CanPlaceOrder();

        // Assert - OrderService 只檢查時間，不檢查週末
        result.Should().BeTrue();
    }

    [Theory]
    [AutoDataWithCustomization]
    public void GetTimeBasedDiscount_週一_應回傳無優惠(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange
        var mondayTime = new DateTime(2024, 3, 11, 14, 0, 0); // 2024/3/11 是週一
        fakeTimeProvider.SetLocalNow(mondayTime);

        // Act
        var discount = sut.GetTimeBasedDiscount();

        // Assert
        discount.Should().Be("無優惠");
    }

    [Theory]
    [AutoDataWithCustomization]
    public void GetTimeBasedDiscount_週五_應回傳九折優惠(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange
        var fridayTime = new DateTime(2024, 3, 15, 14, 0, 0); // 2024/3/15 是週五
        fakeTimeProvider.SetLocalNow(fridayTime);

        // Act
        var discount = sut.GetTimeBasedDiscount();

        // Assert
        discount.Should().Be("週五快樂：九折優惠");
    }

    [Theory]
    [AutoDataWithCustomization]
    public void GetTimeBasedDiscount_週六日_應回傳無優惠(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange - 測試週六
        var saturdayTime = new DateTime(2024, 3, 16, 14, 0, 0); // 2024/3/16 是週六
        fakeTimeProvider.SetLocalNow(saturdayTime);

        // Act
        var saturdayDiscount = sut.GetTimeBasedDiscount();

        // Assert
        saturdayDiscount.Should().Be("無優惠");

        // Arrange - 測試週日
        var sundayTime = new DateTime(2024, 3, 17, 14, 0, 0); // 2024/3/17 是週日
        fakeTimeProvider.SetLocalNow(sundayTime);

        // Act
        var sundayDiscount = sut.GetTimeBasedDiscount();

        // Assert
        sundayDiscount.Should().Be("無優惠");
    }

    [Theory]
    [AutoDataWithCustomization]
    public void GetTimeBasedDiscount_聖誕節_應回傳聖誕特惠(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange
        var christmasTime = new DateTime(2024, 12, 25, 14, 0, 0); // 2024年聖誕節
        fakeTimeProvider.SetLocalNow(christmasTime);

        // Act
        var discount = sut.GetTimeBasedDiscount();

        // Assert
        discount.Should().Be("聖誕特惠：八折優惠");
    }

    [Theory]
    [AutoDataWithCustomization]
    public void CanPlaceOrder_營業時間邊界測試_使用不同實例(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider1,
        OrderService sut1)
    {
        // Arrange & Act & Assert - 上午9點整（營業時間開始）
        fakeTimeProvider1.SetLocalNow(new DateTime(2024, 3, 15, 9, 0, 0));
        sut1.CanPlaceOrder().Should().BeTrue();
    }

    [Theory]
    [AutoDataWithCustomization]
    public void CanPlaceOrder_營業時間結束邊界_應回傳False(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange & Act & Assert - 下午5點整（營業時間結束）
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 17, 0, 0));
        sut.CanPlaceOrder().Should().BeFalse();
    }

    [Theory]
    [AutoDataWithCustomization]
    public void CanPlaceOrder_營業時間前_應回傳False(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange & Act & Assert - 上午8點59分（營業時間前）
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 8, 59, 0));
        sut.CanPlaceOrder().Should().BeFalse();
    }

    [Theory]
    [AutoDataWithCustomization]
    public void CanPlaceOrder_營業時間內最後一分鐘_應回傳True(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange & Act & Assert - 下午4點59分（營業時間內）
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 16, 59, 0));
        sut.CanPlaceOrder().Should().BeTrue();
    }

    /// <summary>
    /// 使用 InlineAutoDataWithCustomization 的示範測試
    /// 結合了參數化測試的靈活性與 AutoFixture 的自動化優勢
    /// </summary>
    [Theory]
    [InlineAutoDataWithCustomization(8, false)]  // 上午8點 - 營業時間前
    [InlineAutoDataWithCustomization(14, true)]  // 下午2點 - 營業時間內
    [InlineAutoDataWithCustomization(18, false)] // 下午6點 - 營業時間後
    public void CanPlaceOrder_使用InlineAutoData_示範測試(
        int hour,
        bool expected,
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        OrderService sut)
    {
        // Arrange
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, hour, 0, 0));

        // Act
        var result = sut.CanPlaceOrder();

        // Assert
        result.Should().Be(expected);
    }
}