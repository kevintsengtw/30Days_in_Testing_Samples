using Day08.Core.Tests.Logging;

namespace Day08.Core.Tests.LoggerTests;

/// <summary>
/// 非同步記錄測試範例
/// </summary>
public class AsyncLoggingTests
{
    [Fact]
    public async Task ProcessAsync_非同步處理_應記錄開始和完成訊息()
    {
        // Arrange
        var mockLogger = new ConcurrentTestLogger<AsyncLoggingService>();
        var service = new AsyncLoggingService(mockLogger);

        // Act
        await service.ProcessAsync("test-data");

        // 等待背景記錄完成
        await Task.Delay(200);

        // Assert
        var logs = mockLogger.GetLogs();
        logs.Count.Should().BeGreaterThanOrEqualTo(1);
        logs.Should().Contain(l => l.Message.Contains("開始處理資料"));
        logs.Should().Contain(l => l.Message.Contains("資料處理完成"));
    }
}

/// <summary>
/// class AsyncLoggingService - 非同步記錄服務（用於測試）
/// </summary>
public class AsyncLoggingService
{
    private readonly ILogger<AsyncLoggingService>? _logger;

    /// <summary>
    /// AsyncLoggingService 建構子
    /// </summary>
    /// <param name="logger">The logger.</param>
    public AsyncLoggingService(ILogger<AsyncLoggingService>? logger = null)
    {
        this._logger = logger;
    }

    /// <summary>
    /// 處理非同步資料
    /// </summary>
    /// <param name="data">要處理的資料</param>
    public async Task ProcessAsync(string data)
    {
        this._logger?.LogInformation("開始處理資料: {Data}", data);

        // 模擬非同步處理
        await Task.Delay(100);

        // 背景記錄
        _ = Task.Run(() => this._logger?.LogInformation("資料處理完成: {Data}", data));
    }
}