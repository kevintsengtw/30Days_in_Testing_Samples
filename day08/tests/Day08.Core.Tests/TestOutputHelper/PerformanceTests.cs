using System.Diagnostics;
using Day08.Core.Services;
using Xunit.Abstractions;

namespace Day08.Core.Tests.TestOutputHelper;

/// <summary>
/// class PerformanceTests - 效能測試與時間點記錄範例
/// </summary>
public class PerformanceTests
{
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// PerformanceTests 建構子
    /// </summary>
    /// <param name="testOutputHelper">測試輸出協助器</param>
    public PerformanceTests(ITestOutputHelper testOutputHelper)
    {
        this._output = testOutputHelper;
    }

    [Fact]
    public async Task ProcessLargeDataSet_處理一萬筆資料_應在五秒內完成()
    {
        // Arrange
        var dataSet = GenerateLargeDataSet(10000);
        var processor = new DataProcessor();

        var stopwatch = Stopwatch.StartNew();
        var checkpoints = new List<(string Stage, TimeSpan Elapsed)>();

        // Act & Monitor
        this._output.WriteLine("開始處理大型資料集...");

        stopwatch.Restart();
        await processor.LoadData(dataSet);
        checkpoints.Add(("資料載入", stopwatch.Elapsed));
        this._output.WriteLine($"資料載入完成: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");

        await processor.ValidateData();
        checkpoints.Add(("資料驗證", stopwatch.Elapsed));
        this._output.WriteLine($"資料驗證完成: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");

        var result = await processor.ProcessData();
        checkpoints.Add(("資料處理", stopwatch.Elapsed));
        this._output.WriteLine($"資料處理完成: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");

        stopwatch.Stop();

        // Assert & Report
        this._output.WriteLine("\n=== 效能報告 ===");
        foreach (var (stage, elapsed) in checkpoints)
        {
            this._output.WriteLine($"{stage}: {elapsed.TotalMilliseconds:F2} ms");
        }

        var totalTime = stopwatch.Elapsed;
        this._output.WriteLine($"總執行時間: {totalTime.TotalMilliseconds:F2} ms");

        // 驗證效能要求（例如：5秒內完成）
        totalTime.Should().BeLessThan(TimeSpan.FromSeconds(5));
        result.Success.Should().BeTrue();
        result.ProcessedCount.Should().Be(10000);
    }

    /// <summary>
    /// 產生大型資料集
    /// </summary>
    /// <param name="count">資料筆數</param>
    /// <returns></returns>
    private static IEnumerable<string> GenerateLargeDataSet(int count)
    {
        return Enumerable.Range(1, count).Select(i => $"Data-{i:D6}");
    }
}