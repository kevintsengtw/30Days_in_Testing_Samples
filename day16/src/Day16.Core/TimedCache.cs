namespace Day16.Core;

/// <summary>
/// 快取項目
/// </summary>
/// <typeparam name="T">快取值的類型</typeparam>
/// <param name="Value">快取的值</param>
/// <param name="ExpiryTime">過期時間</param>
public record CacheItem<T>(T Value, DateTimeOffset ExpiryTime);

/// <summary>
/// 帶有時間過期機制的快取
/// </summary>
/// <typeparam name="T">快取值的類型</typeparam>
public class TimedCache<T>
{
    private readonly TimeProvider _timeProvider;
    private readonly Dictionary<string, CacheItem<T>> _cache = new();

    /// <summary>
    /// 建立定時快取實例
    /// </summary>
    /// <param name="timeProvider">時間提供者</param>
    /// <param name="defaultExpiry">預設過期時間</param>
    public TimedCache(TimeProvider timeProvider, TimeSpan defaultExpiry)
    {
        this._timeProvider = timeProvider;
        this.DefaultExpiry = defaultExpiry;
    }

    /// <summary>
    /// 預設過期時間
    /// </summary>
    public TimeSpan DefaultExpiry { get; }

    /// <summary>
    /// 設定快取項目
    /// </summary>
    /// <param name="key">快取鍵</param>
    /// <param name="value">快取值</param>
    /// <param name="expiry">過期時間（可選，使用預設值）</param>
    public void Set(string key, T value, TimeSpan? expiry = null)
    {
        var expiryTime = this._timeProvider.GetUtcNow().Add(expiry ?? this.DefaultExpiry);
        this._cache[key] = new CacheItem<T>(value, expiryTime);
    }

    /// <summary>
    /// 取得快取項目
    /// </summary>
    /// <param name="key">快取鍵</param>
    /// <returns>快取值，如果不存在或已過期則回傳 default</returns>
    public T? Get(string key)
    {
        if (!this._cache.TryGetValue(key, out var item))
        {
            return default;
        }

        if (item.ExpiryTime <= this._timeProvider.GetUtcNow())
        {
            this._cache.Remove(key);
            return default;
        }

        return item.Value;
    }
}