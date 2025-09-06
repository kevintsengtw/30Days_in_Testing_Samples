namespace TUnit.Demo.Tests;

/// <summary>
/// 展示 TUnit 進階功能的測試類別
/// </summary>
public class TUnitAdvancedTests
{
    #region TimeProvider 時間測試

    [Test]
    public async Task CreateUser_使用FakeTimeProvider_應設定正確的時間戳記()
    {
        // Arrange
        var fakeTime = new DateTime(2023, 12, 25, 14, 30, 0);
        var fakeTimeProvider = new FakeTimeProvider(fakeTime);
        var timeService = new TimeService(fakeTimeProvider);
        var email = "test@example.com";

        // Act
        var user = timeService.CreateUser(email);

        // Assert - 精確時間控制
        await Assert.That(user.CreatedAt).IsEqualTo(fakeTime);
        await Assert.That(user.Email).IsEqualTo(email);
        await Assert.That(user.Id).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task IsBusinessHours_營業時間內_應回傳True()
    {
        // Arrange - 設定為營業時間（下午2點）
        var businessTime = new DateTime(2023, 12, 25, 14, 0, 0);
        var fakeTimeProvider = new FakeTimeProvider(businessTime);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var result = timeService.IsBusinessHours();

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsBusinessHours_非營業時間_應回傳False()
    {
        // Arrange - 設定為非營業時間（晚上8點）
        var afterHours = new DateTime(2023, 12, 25, 20, 0, 0);
        var fakeTimeProvider = new FakeTimeProvider(afterHours);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var result = timeService.IsBusinessHours();

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task GetTimeBasedDiscount_聖誕節_應回傳聖誕特惠()
    {
        // Arrange - 設定為聖誕節
        var christmas = new DateTime(2023, 12, 25);
        var fakeTimeProvider = new FakeTimeProvider(christmas);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var discount = timeService.GetTimeBasedDiscount();

        // Assert
        await Assert.That(discount).IsEqualTo("聖誕特惠：八折優惠");
    }

    [Test]
    public async Task GetTimeBasedDiscount_週五_應回傳週五優惠()
    {
        // Arrange - 設定為週五
        var friday = new DateTime(2023, 12, 22); // 2023年12月22日是週五
        var fakeTimeProvider = new FakeTimeProvider(friday);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var discount = timeService.GetTimeBasedDiscount();

        // Assert
        await Assert.That(discount).IsEqualTo("週五快樂：九折優惠");
    }

    #endregion

    #region 時間範圍驗證

    [Test]
    public async Task GetCurrentTime_應回傳TimeProvider設定的時間()
    {
        // Arrange
        var expectedTime = new DateTime(2023, 10, 15, 9, 30, 45);
        var fakeTimeProvider = new FakeTimeProvider(expectedTime);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var actualTime = timeService.GetCurrentTime();

        // Assert
        await Assert.That(actualTime).IsEqualTo(expectedTime);
    }

    [Test]
    public async Task CalculateElapsed_使用FakeTimeProvider_應計算正確時間差()
    {
        // Arrange
        var startTime = new DateTime(2023, 10, 15, 9, 0, 0);
        var currentTime = new DateTime(2023, 10, 15, 9, 30, 0);
        var fakeTimeProvider = new FakeTimeProvider(currentTime);
        var timeService = new TimeService(fakeTimeProvider);

        // Act
        var elapsed = timeService.CalculateElapsed(startTime);

        // Assert
        await Assert.That(elapsed).IsEqualTo(TimeSpan.FromMinutes(30));
    }

    #endregion

    #region 並行控制測試

    [Test]
    [NotInParallel("DatabaseTests")]
    public async Task 資料庫測試1_不並行執行()
    {
        // 模擬資料庫操作
        await Task.Delay(100);
        var result = 1 + 1;
        await Assert.That(result).IsEqualTo(2);
    }

    [Test]
    [NotInParallel("DatabaseTests")]
    public async Task 資料庫測試2_不並行執行()
    {
        // 模擬資料庫操作
        await Task.Delay(100);
        var result = 2 + 2;
        await Assert.That(result).IsEqualTo(4);
    }

    [Test]
    public async Task 一般測試_可以並行執行()
    {
        await Task.Delay(50);
        var result = 1 + 1;
        await Assert.That(result).IsEqualTo(2);
    }

    #endregion
}