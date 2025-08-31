using Day22.Core.Configuration;

namespace Day22.Core.Services;

/// <summary>
/// Redis 快取服務 - 展示完整的 Redis 操作
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connection;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly RedisSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly TimeProvider _timeProvider;

    public RedisCacheService(
        IConnectionMultiplexer connection,
        IOptions<RedisSettings> settings,
        ILogger<RedisCacheService> logger,
        TimeProvider timeProvider)
    {
        _connection = connection;
        _database = connection.GetDatabase(settings.Value.Database);
        _settings = settings.Value;
        _logger = logger;
        _timeProvider = timeProvider;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };
    }

    #region String Operations

    /// <summary>
    /// 設定字串快取
    /// </summary>
    public async Task<bool> SetStringAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            var actualExpiry = expiry ?? TimeSpan.FromMinutes(_settings.DefaultExpiryMinutes);

            var result = await _database.StringSetAsync(fullKey, serializedValue, actualExpiry);

            if (result)
            {
                _logger.LogDebug("成功設定快取: {Key}", fullKey);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "設定快取失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 取得字串快取
    /// </summary>
    public async Task<T?> GetStringAsync<T>(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var value = await _database.StringGetAsync(fullKey);

            if (!value.HasValue)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得快取失敗: {Key}", key);
            return default;
        }
    }

    /// <summary>
    /// 設定多個字串快取
    /// </summary>
    public async Task<bool> SetMultipleStringAsync<T>(Dictionary<string, T> keyValues, TimeSpan? expiry = null)
    {
        try
        {
            var tasks = keyValues.Select(kvp => SetStringAsync(kvp.Key, kvp.Value, expiry));
            var results = await Task.WhenAll(tasks);
            return results.All(r => r);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批次設定快取失敗");
            return false;
        }
    }

    /// <summary>
    /// 取得多個字串快取
    /// </summary>
    public async Task<Dictionary<string, T?>> GetMultipleStringAsync<T>(IEnumerable<string> keys)
    {
        try
        {
            var fullKeys = keys.Select(GetFullKey).ToArray();
            var redisKeys = fullKeys.Select(k => (RedisKey)k).ToArray();
            var values = await _database.StringGetAsync(redisKeys);

            var result = new Dictionary<string, T?>();
            var keyArray = keys.ToArray();

            for (var i = 0; i < keyArray.Length; i++)
            {
                if (values[i].HasValue)
                {
                    result[keyArray[i]] = JsonSerializer.Deserialize<T>(values[i]!, _jsonOptions);
                }
                else
                {
                    result[keyArray[i]] = default;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批次取得快取失敗");
            return new Dictionary<string, T?>();
        }
    }

    #endregion

    #region Hash Operations

    /// <summary>
    /// 設定 Hash 欄位
    /// </summary>
    public async Task<bool> SetHashAsync<T>(string key, string field, T value)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);

            var result = await _database.HashSetAsync(fullKey, field, serializedValue);
            _logger.LogDebug("成功設定 Hash 快取: {Key}.{Field}", fullKey, field);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "設定 Hash 快取失敗: {Key}.{Field}", key, field);
            return false;
        }
    }

    /// <summary>
    /// 取得 Hash 欄位
    /// </summary>
    public async Task<T?> GetHashAsync<T>(string key, string field)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var value = await _database.HashGetAsync(fullKey, field);

            if (!value.HasValue)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Hash 快取失敗: {Key}.{Field}", key, field);
            return default;
        }
    }

    /// <summary>
    /// 設定整個 Hash
    /// </summary>
    public async Task<bool> SetHashAllAsync<T>(string key, T obj, TimeSpan? expiry = null)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var properties = typeof(T).GetProperties();
            var hashFields = new HashEntry[properties.Length];

            for (var i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                var value = prop.GetValue(obj);
                var serializedValue = value != null ? JsonSerializer.Serialize(value, _jsonOptions) : string.Empty;
                hashFields[i] = new HashEntry(prop.Name.ToLower(), serializedValue);
            }

            await _database.HashSetAsync(fullKey, hashFields);

            if (expiry.HasValue)
            {
                await _database.KeyExpireAsync(fullKey, expiry.Value);
            }

            _logger.LogDebug("成功設定完整 Hash: {Key}", fullKey);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "設定完整 Hash 失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 取得完整 Hash
    /// </summary>
    public async Task<T?> GetHashAllAsync<T>(string key) where T : new()
    {
        try
        {
            var fullKey = GetFullKey(key);
            var hash = await _database.HashGetAllAsync(fullKey);

            if (hash.Length == 0)
            {
                return default;
            }

            var obj = new T();
            var properties = typeof(T).GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);

            foreach (var hashEntry in hash)
            {
                var fieldName = hashEntry.Name.ToString().ToLower();
                if (properties.TryGetValue(fieldName, out var property) && hashEntry.Value.HasValue)
                {
                    var value = JsonSerializer.Deserialize(hashEntry.Value!, property.PropertyType, _jsonOptions);
                    property.SetValue(obj, value);
                }
            }

            return obj;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得完整 Hash 失敗: {Key}", key);
            return default;
        }
    }

    #endregion

    #region List Operations

    /// <summary>
    /// 新增到 List 左側
    /// </summary>
    public async Task<long> ListLeftPushAsync<T>(string key, T value)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.ListLeftPushAsync(fullKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "List 左側新增失敗: {Key}", key);
            return -1;
        }
    }

    /// <summary>
    /// 新增到 List 右側
    /// </summary>
    public async Task<long> ListRightPushAsync<T>(string key, T value)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.ListRightPushAsync(fullKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "List 右側新增失敗: {Key}", key);
            return -1;
        }
    }

    /// <summary>
    /// 從 List 左側取出
    /// </summary>
    public async Task<T?> ListLeftPopAsync<T>(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var value = await _database.ListLeftPopAsync(fullKey);

            if (!value.HasValue)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "List 左側取出失敗: {Key}", key);
            return default;
        }
    }

    /// <summary>
    /// 取得 List 範圍
    /// </summary>
    public async Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var values = await _database.ListRangeAsync(fullKey, start, stop);

            var result = new List<T>();
            foreach (var value in values)
            {
                if (value.HasValue)
                {
                    var item = JsonSerializer.Deserialize<T>(value!, _jsonOptions);
                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 List 範圍失敗: {Key}", key);
            return new List<T>();
        }
    }

    #endregion

    #region Sorted Set Operations

    /// <summary>
    /// 新增到 Sorted Set
    /// </summary>
    public async Task<bool> SortedSetAddAsync<T>(string key, T member, double score)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedMember = JsonSerializer.Serialize(member, _jsonOptions);
            return await _database.SortedSetAddAsync(fullKey, serializedMember, score);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sorted Set 新增失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 取得 Sorted Set 範圍（依分數排序）
    /// </summary>
    public async Task<List<(T Member, double Score)>> SortedSetRangeWithScoresAsync<T>(
        string key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var values = await _database.SortedSetRangeByRankWithScoresAsync(fullKey, start, stop, order);

            var result = new List<(T Member, double Score)>();
            foreach (var value in values)
            {
                var member = JsonSerializer.Deserialize<T>(value.Element!, _jsonOptions);
                if (member != null)
                {
                    result.Add((member, value.Score));
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Sorted Set 範圍失敗: {Key}", key);
            return new List<(T Member, double Score)>();
        }
    }

    /// <summary>
    /// 取得成員在 Sorted Set 中的排名
    /// </summary>
    public async Task<long?> SortedSetRankAsync<T>(string key, T member, Order order = Order.Ascending)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedMember = JsonSerializer.Serialize(member, _jsonOptions);
            return await _database.SortedSetRankAsync(fullKey, serializedMember, order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Sorted Set 排名失敗: {Key}", key);
            return null;
        }
    }

    #endregion

    #region Stream Operations

    /// <summary>
    /// 新增到 Stream
    /// </summary>
    public async Task<RedisValue> StreamAddAsync<T>(string key, T data, string? id = null)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedData = JsonSerializer.Serialize(data, _jsonOptions);

            var nameValuePairs = new NameValueEntry[]
            {
                new("data", serializedData),
                new("timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()),
                new("type", typeof(T).Name)
            };

            return await _database.StreamAddAsync(fullKey, nameValuePairs, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stream 新增失敗: {Key}", key);
            return RedisValue.Null;
        }
    }

    /// <summary>
    /// 讀取 Stream 範圍
    /// </summary>
    public async Task<List<(string Id, T Data, DateTime Timestamp)>> StreamRangeAsync<T>(
        string key, string? start = null, string? end = null, int count = -1)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var entries = await _database.StreamRangeAsync(fullKey, start, end, count);

            var result = new List<(string Id, T Data, DateTime Timestamp)>();
            foreach (var entry in entries)
            {
                var dataField = entry.Values.FirstOrDefault(v => v.Name == "data");
                var timestampField = entry.Values.FirstOrDefault(v => v.Name == "timestamp");

                if (dataField.Value.HasValue)
                {
                    var data = JsonSerializer.Deserialize<T>(dataField.Value!, _jsonOptions);
                    var timestamp = _timeProvider.GetUtcNow().DateTime; // 預設值

                    if (timestampField.Value.HasValue &&
                        long.TryParse(timestampField.Value!, out var unixTimestamp))
                    {
                        timestamp = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).DateTime;
                    }

                    if (data != null)
                    {
                        result.Add((entry.Id!, data, timestamp));
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取 Stream 範圍失敗: {Key}", key);
            return new List<(string Id, T Data, DateTime Timestamp)>();
        }
    }

    #endregion

    #region Set Operations

    /// <summary>
    /// 新增到 Set
    /// </summary>
    public async Task<bool> SetAddAsync<T>(string key, T value)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.SetAddAsync(fullKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Set 新增失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 檢查 Set 成員是否存在
    /// </summary>
    public async Task<bool> SetContainsAsync<T>(string key, T value)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
            return await _database.SetContainsAsync(fullKey, serializedValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查 Set 成員失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 取得 Set 所有成員
    /// </summary>
    public async Task<HashSet<T>> SetMembersAsync<T>(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var values = await _database.SetMembersAsync(fullKey);

            var result = new HashSet<T>();
            foreach (var value in values)
            {
                if (value.HasValue)
                {
                    var item = JsonSerializer.Deserialize<T>(value!, _jsonOptions);
                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得 Set 成員失敗: {Key}", key);
            return new HashSet<T>();
        }
    }

    #endregion

    #region General Operations

    /// <summary>
    /// 刪除快取
    /// </summary>
    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            var result = await _database.KeyDeleteAsync(fullKey);

            if (result)
            {
                _logger.LogDebug("成功刪除快取: {Key}", fullKey);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除快取失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 檢查快取是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            return await _database.KeyExistsAsync(fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查快取存在失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 設定快取過期時間
    /// </summary>
    public async Task<bool> ExpireAsync(string key, TimeSpan expiry)
    {
        try
        {
            var fullKey = GetFullKey(key);
            return await _database.KeyExpireAsync(fullKey, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "設定快取過期時間失敗: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// 取得快取剩餘存活時間
    /// </summary>
    public async Task<TimeSpan?> GetTtlAsync(string key)
    {
        try
        {
            var fullKey = GetFullKey(key);
            return await _database.KeyTimeToLiveAsync(fullKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得快取存活時間失敗: {Key}", key);
            return null;
        }
    }

    /// <summary>
    /// 模糊搜尋快取鍵值
    /// </summary>
    public async Task<List<string>> SearchKeysAsync(string pattern)
    {
        try
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            var fullPattern = GetFullKey(pattern);
            var keys = server.Keys(database: _settings.Database, pattern: fullPattern);

            return keys.Select(key => key.ToString().Substring(_settings.KeyPrefix.Length)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋快取鍵值失敗: {Pattern}", pattern);
            return new List<string>();
        }
    }

    /// <summary>
    /// 清空資料庫
    /// </summary>
    public async Task FlushDatabaseAsync()
    {
        try
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            await server.FlushDatabaseAsync(_settings.Database);
            _logger.LogWarning("已清空 Redis 資料庫 {Database}", _settings.Database);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清空 Redis 資料庫失敗");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 取得完整的快取鍵值
    /// </summary>
    private string GetFullKey(string key)
    {
        return $"{_settings.KeyPrefix}{key}";
    }

    #endregion
}