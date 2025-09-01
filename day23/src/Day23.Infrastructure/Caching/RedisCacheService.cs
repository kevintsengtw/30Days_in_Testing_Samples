using Day23.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Day23.Infrastructure.Caching;

/// <summary>
/// Redis 快取服務實作
/// </summary>
public class RedisCacheService : ICacheService, IDisposable
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisCacheService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
                               ?? throw new ArgumentNullException(nameof(configuration), "Redis 連線字串不能為空");

        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
    }

    /// <summary>
    /// 取得字串快取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        return value.IsNull ? null : value.ToString();
    }

    public async Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        await _database.StringSetAsync(key, value, expiry);
    }

    /// <summary>
    /// 移除快取
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }

    /// <summary>
    /// 透過前綴移除快取
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="cancellationToken"></param>
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints()[0]);
        var keys = server.Keys(pattern: $"{prefix}*");

        var tasks = keys.Select(key => _database.KeyDeleteAsync(key));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 釋放資源 
    /// </summary>
    public void Dispose()
    {
        _redis?.Dispose();
    }
}