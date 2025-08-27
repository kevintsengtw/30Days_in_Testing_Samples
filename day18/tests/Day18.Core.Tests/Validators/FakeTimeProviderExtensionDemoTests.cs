using Day18.Core.Tests.Extensions;

namespace Day18.Core.Tests.Validators;

/// <summary>
/// 展示 SetLocalNow 擴充方法的使用範例
/// </summary>
public class FakeTimeProviderExtensionDemoTests
{
    [Fact]
    public void 展示SetLocalNow擴充方法的使用()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();

        // 使用擴充方法設定本地時間，語法更直觀
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 14, 0, 0)); // 下午 2 點

        var validator = new UserRegistrationValidator(fakeTimeProvider);
        var request = new UserRegistrationRequest
        {
            Username = "testuser123",
            Email = "test@example.com",
            Password = "TestPass123",
            ConfirmPassword = "TestPass123",
            BirthDate = new DateTime(1990, 1, 1),
            Age = 34, // 2024 - 1990 = 34
            PhoneNumber = "0912345678",
            Roles = ["User"],
            AgreeToTerms = true
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();

        // 驗證時間設定是否正確
        var currentTime = fakeTimeProvider.GetLocalNow();
        currentTime.Year.Should().Be(2024);
        currentTime.Month.Should().Be(3);
        currentTime.Day.Should().Be(15);
        currentTime.Hour.Should().Be(14);
    }

    [Theory]
    [InlineData(2024, 3, 15, 10, 0, 0)]    // 上午 10 點
    [InlineData(2024, 12, 25, 15, 30, 45)] // 聖誕節下午 3:30:45
    [InlineData(2023, 7, 4, 0, 0, 0)]      // 美國國慶日午夜
    public void SetLocalNow_可以設定各種本地時間(int year, int month, int day, int hour, int minute, int second)
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var expectedDateTime = new DateTime(year, month, day, hour, minute, second);

        // Act
        fakeTimeProvider.SetLocalNow(expectedDateTime);

        // Assert
        var actualDateTime = fakeTimeProvider.GetLocalNow().DateTime;
        actualDateTime.Should().Be(expectedDateTime);
    }

    [Fact]
    public void 比較SetLocalNow與SetUtcNow的差異()
    {
        // Arrange
        var timeProviderWithSetLocal = new FakeTimeProvider();
        var timeProviderWithSetUtc = new FakeTimeProvider();
        var localTime = new DateTime(2024, 6, 15, 14, 30, 0); // 下午 2:30

        // Act - 使用 SetLocalNow 擴充方法
        timeProviderWithSetLocal.SetLocalNow(localTime);

        // Act - 使用傳統的 SetUtcNow 方法
        timeProviderWithSetUtc.SetLocalTimeZone(TimeZoneInfo.Local);
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, TimeZoneInfo.Local);
        timeProviderWithSetUtc.SetUtcNow(utcTime);

        // Assert - 兩種方法應該產生相同的結果
        var resultFromSetLocal = timeProviderWithSetLocal.GetLocalNow().DateTime;
        var resultFromSetUtc = timeProviderWithSetUtc.GetLocalNow().DateTime;

        resultFromSetLocal.Should().Be(resultFromSetUtc);
        resultFromSetLocal.Should().Be(localTime);
    }
}