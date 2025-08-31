namespace Day22.Core.Services;

/// <summary>
/// 快取服務介面
/// </summary>
public interface ICacheService
{
    // String Operations

    /// <summary>
    /// 設定字串快取值。
    /// </summary>
    Task<bool> SetStringAsync<T>(string key, T value, TimeSpan? expiry = null);

    /// <summary>
    /// 取得字串快取值。
    /// </summary>
    Task<T?> GetStringAsync<T>(string key);

    /// <summary>
    /// 批次設定多個字串快取值。
    /// </summary>
    Task<bool> SetMultipleStringAsync<T>(Dictionary<string, T> keyValues, TimeSpan? expiry = null);

    /// <summary>
    /// 批次取得多個字串快取值。
    /// </summary>
    Task<Dictionary<string, T?>> GetMultipleStringAsync<T>(IEnumerable<string> keys);

    // Hash Operations

    /// <summary>
    /// 設定雜湊欄位值。
    /// </summary>
    Task<bool> SetHashAsync<T>(string key, string field, T value);

    /// <summary>
    /// 取得雜湊欄位值。
    /// </summary>
    Task<T?> GetHashAsync<T>(string key, string field);

    /// <summary>
    /// 設定整個雜湊物件。
    /// </summary>
    Task<bool> SetHashAllAsync<T>(string key, T obj, TimeSpan? expiry = null);

    /// <summary>
    /// 取得整個雜湊物件。
    /// </summary>
    Task<T?> GetHashAllAsync<T>(string key) where T : new();

    // List Operations

    /// <summary>
    /// 將值推入清單左側。
    /// </summary>
    Task<long> ListLeftPushAsync<T>(string key, T value);

    /// <summary>
    /// 將值推入清單右側。
    /// </summary>
    Task<long> ListRightPushAsync<T>(string key, T value);

    /// <summary>
    /// 從清單左側彈出一個值。
    /// </summary>
    Task<T?> ListLeftPopAsync<T>(string key);

    /// <summary>
    /// 取得清單範圍內的值。
    /// </summary>
    Task<List<T>> ListRangeAsync<T>(string key, long start = 0, long stop = -1);

    // Sorted Set Operations

    /// <summary>
    /// 新增排序集合成員。
    /// </summary>
    Task<bool> SortedSetAddAsync<T>(string key, T member, double score);

    /// <summary>
    /// 取得排序集合指定範圍及分數的成員。
    /// </summary>
    Task<List<(T Member, double Score)>> SortedSetRangeWithScoresAsync<T>(string key, long start = 0, long stop = -1, Order order = Order.Ascending);

    /// <summary>
    /// 取得排序集合成員的排名。
    /// </summary>
    Task<long?> SortedSetRankAsync<T>(string key, T member, Order order = Order.Ascending);

    // Stream Operations

    /// <summary>
    /// 新增資料至資料流。
    /// </summary>
    Task<RedisValue> StreamAddAsync<T>(string key, T data, string? id = null);

    /// <summary>
    /// 取得資料流範圍內的資料。
    /// </summary>
    Task<List<(string Id, T Data, DateTime Timestamp)>> StreamRangeAsync<T>(string key, string? start = null, string? end = null, int count = -1);

    // Set Operations

    /// <summary>
    /// 新增集合成員。
    /// </summary>
    Task<bool> SetAddAsync<T>(string key, T value);

    /// <summary>
    /// 檢查集合是否包含指定成員。
    /// </summary>
    Task<bool> SetContainsAsync<T>(string key, T value);

    /// <summary>
    /// 取得集合所有成員。
    /// </summary>
    Task<HashSet<T>> SetMembersAsync<T>(string key);

    // General Operations

    /// <summary>
    /// 刪除指定快取鍵。
    /// </summary>
    Task<bool> DeleteAsync(string key);

    /// <summary>
    /// 檢查快取鍵是否存在。
    /// </summary>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 設定快取鍵的過期時間。
    /// </summary>
    Task<bool> ExpireAsync(string key, TimeSpan expiry);

    /// <summary>
    /// 取得快取鍵的剩餘存活時間。
    /// </summary>
    Task<TimeSpan?> GetTtlAsync(string key);

    /// <summary>
    /// 依模式搜尋快取鍵。
    /// </summary>
    Task<List<string>> SearchKeysAsync(string pattern);

    /// <summary>
    /// 清空整個資料庫。
    /// </summary>
    Task FlushDatabaseAsync();
}