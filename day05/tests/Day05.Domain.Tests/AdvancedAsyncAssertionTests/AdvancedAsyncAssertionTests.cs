using Day05.Domain.Services.ProcessingServices;

namespace Day05.Domain.Tests.AdvancedAsyncAssertionTests;

public class AdvancedAsyncAssertionTests
{
    [Fact]
    public async Task AsyncAssertion_執行時間_應正常運作()
    {
        // Arrange
        var service = new SlowService();

        // Act
        var asyncAction = () => service.ProcessAsync();

        // Assert
        // 確保非同步操作不會拋出異常
        await asyncAction.Should().NotThrowAsync();

        // 測量執行時間
        var stopwatch = Stopwatch.StartNew();
        await service.ProcessAsync();
        stopwatch.Stop();

        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task AsyncAssertion_CancellationToken_應正常運作()
    {
        // Arrange
        var service = new CancellableService();
        var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        // Act
        var asyncAction = () => service.LongRunningOperationAsync(cts.Token);

        // Assert
        // 確保非同步操作會因取消而拋出 OperationCanceledException
        await asyncAction.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task AsyncAssertion_並行處理_應正常運作()
    {
        // Arrange
        var service = new ParallelService();
        var tasks = Enumerable.Range(1, 10)
                              .Select(i => service.ProcessItemAsync(i))
                              .ToArray();

        // Act
        await Task.WhenAll(tasks);

        // Assert
        tasks.Should().AllSatisfy(task =>
        {
            task.Should().NotBeNull();
            task.Status.Should().Be(TaskStatus.RanToCompletion);
            task.Result.Should().NotBeNull();
        });
    }
}