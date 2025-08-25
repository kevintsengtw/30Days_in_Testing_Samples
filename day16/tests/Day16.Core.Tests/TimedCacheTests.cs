namespace Day16.Core.Tests;

/// <summary>
/// TimedCache 的測試
/// </summary>
public class TimedCacheTests
{
    [Fact]
    public void Cache_設定項目後立即取得_應回傳正確值()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 10, 0, 0));

        var cache = new TimedCache<string>(fakeTimeProvider, TimeSpan.FromMinutes(5));

        // Act
        cache.Set("key1", "value1");
        var result = cache.Get("key1");

        // Assert
        result.Should().Be("value1");
    }

    [Fact]
    public void Cache_設定項目後快轉時間_應正確處理過期()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var startTime = new DateTime(2024, 3, 15, 10, 0, 0);
        fakeTimeProvider.SetLocalNow(startTime);

        var cache = new TimedCache<string>(fakeTimeProvider, TimeSpan.FromMinutes(5));

        // Act & Assert - 設定快取項目（時間點：10:00）
        cache.Set("key1", "value1");
        cache.Get("key1").Should().Be("value1");

        // 模擬時間前進 3 分鐘（時間點：10:03），快取尚未過期（5分鐘期限）
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(3));
        cache.Get("key1").Should().Be("value1"); // 3 < 5，仍在有效期內

        // 再次模擬時間前進 3 分鐘（時間點：10:06），快取已過期
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(3)); // 總計 6 分鐘 > 5 分鐘期限
        cache.Get("key1").Should().BeNull();               // 已過期，返回 null
    }

    [Fact]
    public void Cache_使用自訂過期時間_應按指定時間過期()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 10, 0, 0));

        var cache = new TimedCache<string>(fakeTimeProvider, TimeSpan.FromMinutes(10));

        // Act - 設定自訂過期時間為 2 分鐘
        cache.Set("key1", "value1", TimeSpan.FromMinutes(2));

        // 快轉 1 分鐘，項目應該還在
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(1));
        cache.Get("key1").Should().Be("value1");

        // 再快轉 2 分鐘（總共 3 分鐘），項目應該過期
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(2));

        // Assert
        cache.Get("key1").Should().BeNull();
    }

    [Fact]
    public void Cache_取得不存在的鍵_應回傳Null()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 10, 0, 0));

        var cache = new TimedCache<string>(fakeTimeProvider, TimeSpan.FromMinutes(5));

        // Act
        var result = cache.Get("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Cache_多個項目不同過期時間_應正確處理()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetLocalNow(new DateTime(2024, 3, 15, 10, 0, 0));

        var cache = new TimedCache<string>(fakeTimeProvider, TimeSpan.FromMinutes(10));

        // Act - 設定不同過期時間的項目
        cache.Set("short", "value1", TimeSpan.FromMinutes(2));
        cache.Set("long", "value2", TimeSpan.FromMinutes(8));

        // 快轉 3 分鐘
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(3));

        // Assert - 短期項目應過期，長期項目應存在
        cache.Get("short").Should().BeNull();
        cache.Get("long").Should().Be("value2");

        // 再快轉 6 分鐘（總共 9 分鐘）
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(6));

        // Assert - 長期項目也應過期
        cache.Get("long").Should().BeNull();
    }

    [Fact]
    public void DefaultExpiry_應正確設定()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var expectedExpiry = TimeSpan.FromMinutes(15);

        // Act
        var cache = new TimedCache<string>(fakeTimeProvider, expectedExpiry);

        // Assert
        cache.DefaultExpiry.Should().Be(expectedExpiry);
    }

    [Theory]
    [AutoDataWithCustomization]
    public void TimedCache_使用AutoFixture測試過期機制_應正確處理(
        [Frozen(Matching.DirectBaseType)] FakeTimeProvider fakeTimeProvider,
        string key,
        string value)
    {
        // Arrange
        var startTime = new DateTime(2024, 3, 15, 10, 0, 0);
        fakeTimeProvider.SetLocalNow(startTime);

        var cache = new TimedCache<string>(fakeTimeProvider, TimeSpan.FromMinutes(30));

        // Act & Assert - 設定和立即取得
        cache.Set(key, value);
        cache.Get(key).Should().Be(value);

        // Act & Assert - 快轉時間後應過期
        fakeTimeProvider.Advance(TimeSpan.FromMinutes(31));
        cache.Get(key).Should().BeNull();
    }
}