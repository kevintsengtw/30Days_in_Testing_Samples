namespace Day16.Core.Tests;

/// <summary>
/// ScheduleService 的測試
/// </summary>
public class ScheduleServiceTests
{
    [Theory]
    [InlineData("2024-03-15 14:30:00", "2024-03-15 14:00:00", true)]  // 已到執行時間
    [InlineData("2024-03-15 13:30:00", "2024-03-15 14:00:00", false)] // 尚未到執行時間
    public void ShouldExecuteJob_根據時間判斷_應回傳正確結果(
        string currentTimeStr,
        string scheduledTimeStr,
        bool expected)
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var currentTime = DateTime.Parse(currentTimeStr);
        var scheduledTime = DateTime.Parse(scheduledTimeStr);

        fakeTimeProvider.SetLocalNow(currentTime);

        var schedule = new JobSchedule { NextExecutionTime = scheduledTime };
        var service = new ScheduleService(fakeTimeProvider);

        // Act
        var result = service.ShouldExecuteJob(schedule);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CalculateNextExecution_每日午夜排程_應回傳明天午夜()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var currentTime = new DateTime(2024, 3, 15, 14, 30, 0);
        fakeTimeProvider.SetLocalNow(currentTime);

        var schedule = new JobSchedule { CronExpression = "0 0 * * *" };
        var service = new ScheduleService(fakeTimeProvider);

        // Act
        var result = service.CalculateNextExecution(schedule);

        // Assert
        var expectedNextExecution = new DateTime(2024, 3, 16, 0, 0, 0); // 明天午夜
        result.Should().Be(expectedNextExecution);
    }

    [Fact]
    public void CalculateNextExecution_每週一排程_當前是週三_應回傳下週一()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var currentTime = new DateTime(2024, 3, 13, 14, 30, 0); // 2024/3/13 是週三
        fakeTimeProvider.SetLocalNow(currentTime);

        var schedule = new JobSchedule { CronExpression = "0 0 * * 1" };
        var service = new ScheduleService(fakeTimeProvider);

        // Act
        var result = service.CalculateNextExecution(schedule);

        // Assert
        var expectedNextExecution = new DateTime(2024, 3, 18, 0, 0, 0); // 下週一
        result.Should().Be(expectedNextExecution);
    }

    [Fact]
    public void CalculateNextExecution_每週一排程_當前是週一_應回傳下週一()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var currentTime = new DateTime(2024, 3, 18, 14, 30, 0); // 2024/3/18 是週一
        fakeTimeProvider.SetLocalNow(currentTime);

        var schedule = new JobSchedule { CronExpression = "0 0 * * 1" };
        var service = new ScheduleService(fakeTimeProvider);

        // Act
        var result = service.CalculateNextExecution(schedule);

        // Assert
        var expectedNextExecution = new DateTime(2024, 3, 25, 0, 0, 0); // 下週一
        result.Should().Be(expectedNextExecution);
    }

    [Fact]
    public void CalculateNextExecution_預設排程_應回傳一小時後()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var currentTime = new DateTime(2024, 3, 15, 14, 30, 0);
        fakeTimeProvider.SetLocalNow(currentTime);

        var schedule = new JobSchedule { CronExpression = "unknown" };
        var service = new ScheduleService(fakeTimeProvider);

        // Act
        var result = service.CalculateNextExecution(schedule);

        // Assert
        var expectedNextExecution = new DateTime(2024, 3, 15, 15, 30, 0); // 一小時後
        result.Should().Be(expectedNextExecution);
    }
}