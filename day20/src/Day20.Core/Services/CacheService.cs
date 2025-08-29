using System.Text.Json;

namespace Day20.Core.Services;

/// <summary>
/// 快取服務介面
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 取得快取值
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// 設定快取值
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;

    /// <summary>
    /// 刪除快取值
    /// </summary>
    Task RemoveAsync(string key);

    /// <summary>
    /// 檢查快取鍵是否存在
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 清除所有快取
    /// </summary>
    Task ClearAllAsync();
}

/// <summary>
/// Redis 快取服務實作
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    /// <summary>
    /// 取得快取值
    /// </summary>
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var value = await _database.StringGetAsync(key);
        if (!value.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(value!);
    }

    /// <summary>
    /// 設定快取值
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serializedValue, expiry);
    }

    /// <summary>
    /// 刪除快取值
    /// </summary>
    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    /// <summary>
    /// 檢查快取鍵是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    /// <summary>
    /// 清除所有快取
    /// </summary>
    public async Task ClearAllAsync()
    {
        var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
        await server.FlushDatabaseAsync();
    }
}