using Day22.Core.Extensions;
using Day22.Integration.Tests.Fixtures;
using Microsoft.Extensions.Time.Testing;
using Xunit;
using AwesomeAssertions;

namespace Day22.Integration.Tests.Extensions;

/// <summary>
/// DateTime 擴充方法測試
/// </summary>
public class DateTimeExtensionsTests
{
    [Fact]
    public void CalculateAge_使用系統時間_應正確計算年齡()
    {
        // Arrange
        var birthDate = new DateTime(1990, 6, 15);

        // Act
        var age = birthDate.CalculateAge();

        // Assert
        var expectedAge = DateTime.Today.Year - 1990;
        if (new DateTime(1990, 6, 15) > DateTime.Today.AddYears(-expectedAge))
        {
            expectedAge--;
        }

        age.Should().Be(expectedAge);
    }

    [Fact]
    public void CalculateAge_使用FakeTimeProvider_應正確計算年齡()
    {
        // Arrange
        var birthDate = new DateTime(1990, 6, 15);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2024, 8, 31, 0, 0, 0, TimeSpan.Zero));

        // Act
        var age = birthDate.CalculateAge(fakeTimeProvider);

        // Assert
        age.Should().Be(34); // 2024 - 1990 = 34，且生日已過
    }

    [Fact]
    public void CalculateAge_生日未到_應減少一歲()
    {
        // Arrange
        var birthDate = new DateTime(1990, 12, 25); // 聖誕節生日
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero)); // 6月，生日未到

        // Act
        var age = birthDate.CalculateAge(fakeTimeProvider);

        // Assert
        age.Should().Be(33); // 2024 - 1990 - 1 = 33，因為生日未到
    }

    [Fact]
    public void CalculateAge_生日當天_應正確計算年齡()
    {
        // Arrange
        var birthDate = new DateTime(1990, 8, 31);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(2024, 8, 31, 0, 0, 0, TimeSpan.Zero)); // 生日當天

        // Act
        var age = birthDate.CalculateAge(fakeTimeProvider);

        // Assert
        age.Should().Be(34); // 2024 - 1990 = 34，生日當天算滿歲
    }

    [Theory]
    [InlineData("1990-01-01", "2024-01-01", 34)]
    [InlineData("1990-01-01", "2023-12-31", 33)]
    [InlineData("1985-05-15", "2024-05-15", 39)]
    [InlineData("1985-05-15", "2024-05-14", 38)]
    [InlineData("2000-02-29", "2024-02-28", 23)] // 閏年測試
    [InlineData("2000-02-29", "2024-03-01", 24)]
    public void CalculateAge_多種日期組合_應正確計算年齡(string birthDateStr, string currentDateStr, int expectedAge)
    {
        // Arrange
        var birthDate = DateTime.Parse(birthDateStr);
        var currentDate = DateTime.Parse(currentDateStr);
        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(currentDate));

        // Act
        var age = birthDate.CalculateAge(fakeTimeProvider);

        // Assert
        age.Should().Be(expectedAge);
    }
}
