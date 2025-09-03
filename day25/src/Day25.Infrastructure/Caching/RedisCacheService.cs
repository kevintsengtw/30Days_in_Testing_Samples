namespace Day25.Infrastructure.Caching;

/// <summary>
/// Redis 快取服務實作
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _database = redis.GetDatabase();
        _logger = logger;
    }

    /// <summary>
    /// 取得快取值
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (!value.HasValue)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得快取失敗，Key: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// 設定快取值
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, serializedValue, expiry);

            _logger.LogDebug("已設定快取，Key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "設定快取失敗，Key: {Key}", key);
        }
    }

    /// <summary>
    /// 移除快取值
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogDebug("已移除快取，Key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除快取失敗，Key: {Key}", key);
        }
    }

    /// <summary>
    /// 移除符合模式的快取值
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);

            var keyArray = keys.ToArray();
            if (keyArray.Length > 0)
            {
                await _database.KeyDeleteAsync(keyArray);
                _logger.LogDebug("已移除 {Count} 個符合模式 {Pattern} 的快取", keyArray.Length, pattern);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "移除符合模式的快取失敗，Pattern: {Pattern}", pattern);
        }
    }
}