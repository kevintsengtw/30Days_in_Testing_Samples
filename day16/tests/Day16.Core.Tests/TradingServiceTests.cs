namespace Day16.Core.Tests;

/// <summary>
/// TradingService 的測試
/// </summary>
public class TradingServiceTests
{
    [Theory]
    [InlineData("09:30:00", true)]  // 上午交易時間
    [InlineData("11:15:00", true)]  // 上午交易時間結束前
    [InlineData("11:30:00", true)]  // 上午交易時間結束點
    [InlineData("12:00:00", false)] // 中午休息時間
    [InlineData("13:00:00", true)]  // 下午交易時間開始
    [InlineData("14:30:00", true)]  // 下午交易時間
    [InlineData("15:00:00", true)]  // 下午交易時間結束點
    [InlineData("15:30:00", false)] // 下午交易結束後
    [InlineData("08:30:00", false)] // 交易開始前
    public void IsInTradingHours_不同時間點_應回傳正確結果(string timeStr, bool expected)
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var testTime = DateTime.Today.Add(TimeSpan.Parse(timeStr));
        fakeTimeProvider.SetLocalNow(testTime);

        var service = new TradingService(fakeTimeProvider);

        // Act
        var result = service.IsInTradingHours();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(DayOfWeek.Monday, 10, 1.0)]    // 週一正常時間
    [InlineData(DayOfWeek.Tuesday, 10, 1.0)]   // 週二正常時間
    [InlineData(DayOfWeek.Wednesday, 10, 1.0)] // 週三正常時間
    [InlineData(DayOfWeek.Thursday, 10, 1.0)]  // 週四正常時間
    [InlineData(DayOfWeek.Friday, 10, 1.0)]    // 週五上午
    [InlineData(DayOfWeek.Friday, 14, 1.1)]    // 週五下午
    [InlineData(DayOfWeek.Friday, 15, 1.1)]    // 週五下午晚些時候
    [InlineData(DayOfWeek.Saturday, 10, 0.0)]  // 週六
    [InlineData(DayOfWeek.Sunday, 10, 0.0)]    // 週日
    public void GetMarketMultiplier_不同日期和時間_應回傳正確乘數(DayOfWeek dayOfWeek, int hour, decimal expected)
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();

        // 計算指定星期幾的日期
        var baseDate = new DateTime(2024, 3, 11); // 2024/3/11 是週一
        var daysToAdd = (int)dayOfWeek - (int)baseDate.DayOfWeek;
        if (daysToAdd < 0)
        {
            daysToAdd += 7;
        }

        var testTime = baseDate.AddDays(daysToAdd).AddHours(hour);
        fakeTimeProvider.SetLocalNow(testTime);

        var service = new TradingService(fakeTimeProvider);

        // Act
        var result = service.GetMarketMultiplier();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsInTradingHours_邊界測試_上午時段邊界()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var service = new TradingService(fakeTimeProvider);

        // Act & Assert - 9:00 開始交易
        fakeTimeProvider.SetLocalNow(DateTime.Today.AddHours(9));
        service.IsInTradingHours().Should().BeTrue();

        // Act & Assert - 11:30 結束交易
        fakeTimeProvider.SetLocalNow(DateTime.Today.AddHours(11).AddMinutes(30));
        service.IsInTradingHours().Should().BeTrue();

        // Act & Assert - 11:31 已結束交易
        fakeTimeProvider.SetLocalNow(DateTime.Today.AddHours(11).AddMinutes(31));
        service.IsInTradingHours().Should().BeFalse();
    }

    [Fact]
    public void IsInTradingHours_邊界測試_下午時段邊界()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var service = new TradingService(fakeTimeProvider);

        // Act & Assert - 13:00 開始交易
        fakeTimeProvider.SetLocalNow(DateTime.Today.AddHours(13));
        service.IsInTradingHours().Should().BeTrue();

        // Act & Assert - 15:00 結束交易
        fakeTimeProvider.SetLocalNow(DateTime.Today.AddHours(15));
        service.IsInTradingHours().Should().BeTrue();

        // Act & Assert - 15:01 已結束交易
        fakeTimeProvider.SetLocalNow(DateTime.Today.AddHours(15).AddMinutes(1));
        service.IsInTradingHours().Should().BeFalse();
    }
}