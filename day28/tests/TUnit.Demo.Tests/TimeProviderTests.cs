namespace TUnit.Demo.Tests;

/// <summary>
/// 展示 TimeProvider 和 FakeTimeProvider 測試的範例
/// </summary>
public class TimeProviderTests
{
    [Test]
    public async Task CreateUser_應設定正確的時間戳記()
    {
        // Arrange
        var fakeTime = new DateTimeOffset(2024, 12, 25, 10, 30, 0, TimeSpan.Zero);
        var fakeTimeProvider = new FakeTimeProvider(fakeTime);
        var timeService = new TimeService(fakeTimeProvider);
        var email = "test@example.com";

        // Act
        var user = timeService.CreateUser(email);

        // Assert
        await Assert.That(user.CreatedAt).IsEqualTo(fakeTime.DateTime);
        await Assert.That(user.Email).IsEqualTo(email);
        await Assert.That(user.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task CalculateElapsed_應計算正確的時間差()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var timeService = new TimeService(fakeTimeProvider);
        var startTime = fakeTimeProvider.GetLocalNow().DateTime;

        // Act - 前進 2 小時
        fakeTimeProvider.Advance(TimeSpan.FromHours(2));
        var elapsed = timeService.CalculateElapsed(startTime);

        // Assert
        await Assert.That(elapsed).IsEqualTo(TimeSpan.FromHours(2));
    }

    [Test]
    [Arguments(9, true)]   // 9:00 AM - 營業時間
    [Arguments(12, true)]  // 12:00 PM - 營業時間
    [Arguments(16, true)]  // 4:00 PM - 營業時間
    [Arguments(8, false)]  // 8:00 AM - 非營業時間
    [Arguments(17, false)] // 5:00 PM - 非營業時間
    [Arguments(22, false)] // 10:00 PM - 非營業時間
    public async Task IsBusinessHours_各時段檢查_應回傳正確結果(int hour, bool expected)
    {
        // Arrange
        var testTime = new DateTimeOffset(2024, 6, 15, hour, 0, 0, TimeSpan.Zero);
        var fakeTimeProvider = new FakeTimeProvider(testTime);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var result = timeService.IsBusinessHours();

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task GetTimeBasedDiscount_週五_應回傳週五優惠()
    {
        // Arrange - 設定為週五
        var friday = new DateTimeOffset(2024, 6, 14, 12, 0, 0, TimeSpan.Zero); // 2024/6/14 是週五
        var fakeTimeProvider = new FakeTimeProvider(friday);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var discount = timeService.GetTimeBasedDiscount();

        // Assert
        await Assert.That(discount).IsEqualTo("週五快樂：九折優惠");
    }

    [Test]
    public async Task GetTimeBasedDiscount_聖誕節_應回傳聖誕優惠()
    {
        // Arrange - 設定為聖誕節
        var christmas = new DateTimeOffset(2024, 12, 25, 12, 0, 0, TimeSpan.Zero);
        var fakeTimeProvider = new FakeTimeProvider(christmas);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var discount = timeService.GetTimeBasedDiscount();

        // Assert
        await Assert.That(discount).IsEqualTo("聖誕特惠：八折優惠");
    }

    [Test]
    public async Task GetTimeBasedDiscount_一般日期_應回傳無優惠()
    {
        // Arrange - 設定為一般週二
        var normalDay = new DateTimeOffset(2024, 6, 11, 12, 0, 0, TimeSpan.Zero); // 2024/6/11 是週二
        var fakeTimeProvider = new FakeTimeProvider(normalDay);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var discount = timeService.GetTimeBasedDiscount();

        // Assert
        await Assert.That(discount).IsEqualTo("無優惠");
    }
}