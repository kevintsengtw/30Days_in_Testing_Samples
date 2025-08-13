using Day04.Domain.Services;

namespace Day04.Domain.Tests.BasicAssertions;

/// <summary>
/// class AsyncAssertionTests - 非同步斷言測試
/// </summary>
public class AsyncAssertionTests
{
    private readonly UserService _userService = new();

    public AsyncAssertionTests()
    {
        // 準備測試資料
        this._userService.CreateUser("test@example.com", "Test User");
    }

    [Fact]
    public async Task IsCompletedSuccessfully_非同步任務_應成功完成()
    {
        // Task 完成狀態斷言
        var task = this._userService.GetUserAsync(1);

        var result = await task; // 等待完成

        task.IsCompletedSuccessfully.Should().BeTrue();
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task ThrowAsync_非同步異常_應拋出ArgumentException()
    {
        // 測試異常情況
        var task = async () => await this._userService.GetUserAsync(-1);

        await task.Should().ThrowAsync<ArgumentException>()
                  .WithMessage("*使用者 ID 必須為正數*");
    }

    [Fact]
    public async Task ThrowAsync_任務取消_應拋出TaskCanceledException()
    {
        // 模擬逾時情況
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        var act = async () => await this._userService.GetUserWithDelayAsync(1, cancellationTokenSource.Token);

        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task WhenAll_多個並行任務_應全部成功完成()
    {
        // 測試多個非同步任務
        var tasks = new[]
        {
            this._userService.GetUserAsync(1),
            this._userService.GetUserAsync(1),
            this._userService.GetUserAsync(1)
        };

        var results = await Task.WhenAll(tasks);

        results.Should().HaveCount(3);
        results.Should().OnlyContain(user => user.Id == 1);
    }
}