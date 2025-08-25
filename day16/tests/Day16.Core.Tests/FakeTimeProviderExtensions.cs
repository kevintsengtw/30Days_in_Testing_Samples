namespace Day16.Core.Tests;

/// <summary>
/// FakeTimeProvider 的擴充方法
/// </summary>
public static class FakeTimeProviderExtensions
{
    /// <summary>
    /// 設定 FakeTimeProvider 的本地時間
    /// </summary>
    /// <param name="fakeTimeProvider">FakeTimeProvider 實例</param>
    /// <param name="localDateTime">要設定的本地時間</param>
    public static void SetLocalNow(this FakeTimeProvider fakeTimeProvider, DateTime localDateTime)
    {
        fakeTimeProvider.SetLocalTimeZone(TimeZoneInfo.Local);
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo.Local);
        fakeTimeProvider.SetUtcNow(utcTime);
    }
}