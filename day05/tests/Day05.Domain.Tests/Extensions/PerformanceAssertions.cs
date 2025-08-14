namespace Day05.Domain.Tests.Extensions;

// 大量資料比對的效能優化策略
public static class PerformanceOptimizedAssertions
{
    // 選擇性屬性比較，避免全物件掃描
    public static void AssertLargeDataSet<T>(IEnumerable<T> actual, IEnumerable<T> expected)
    {
        actual.Should().HaveCount(expected.Count());

        // 分批處理，避免記憶體壓力
        var actualBatches = actual.Chunk(1000);
        var expectedBatches = expected.Chunk(1000);

        actualBatches.Zip(expectedBatches, (a, e) => new { Actual = a, Expected = e })
                     .AsParallel()
                     .ForAll(batch =>
                     {
                         batch.Actual.Should().BeEquivalentTo(
                             batch.Expected,
                             options => options.Excluding(ctx => ctx.Path.Contains("UpdatedAt"))
                                               .Excluding(ctx => ctx.Path.Contains("CreatedAt")));
                     });
    }

    // 關鍵屬性快速比對
    public static void AssertKeyPropertiesOnly<T>(T actual, T expected, params Expression<Func<T, object>>[] keySelectors)
    {
        foreach (var selector in keySelectors)
        {
            var actualValue = selector.Compile()(actual);
            var expectedValue = selector.Compile()(expected);
            actualValue.Should().Be(expectedValue, $"關鍵屬性 {selector} 應該相符");
        }
    }
}