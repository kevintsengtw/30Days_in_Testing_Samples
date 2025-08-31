using System;
using System.Text.Json.Serialization;

namespace Day22.Core.Models.Redis;

/// <summary>
/// 快取項目包裝器 - 提供過期時間和元資料管理
/// </summary>
/// <typeparam name="T">快取資料型別</typeparam>
public class CacheItem<T>
{
    /// <summary>
    /// 快取的資料
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; } = default!;

    /// <summary>
    /// 建立時間
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 過期時間
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// 快取鍵值
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 標籤，用於分群管理
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// 存取次數
    /// </summary>
    [JsonPropertyName("access_count")]
    public int AccessCount { get; set; }

    /// <summary>
    /// 最後存取時間
    /// </summary>
    [JsonPropertyName("last_accessed")]
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 快取項目版本
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// 額外的元資料
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// 檢查是否已過期
    /// </summary>
    [JsonIgnore]
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;

    /// <summary>
    /// 計算存活時間（秒）
    /// </summary>
    [JsonIgnore]
    public double TtlSeconds => ExpiresAt.HasValue
        ? Math.Max(0, ExpiresAt.Value.Subtract(DateTime.UtcNow).TotalSeconds)
        : -1;

    /// <summary>
    /// 建立快取項目
    /// </summary>
    /// <param name="key">快取鍵值</param>
    /// <param name="data">快取資料</param>
    /// <param name="ttl">存活時間</param>
    /// <param name="tags">標籤</param>
    /// <returns>快取項目</returns>
    public static CacheItem<T> Create(string key, T data, TimeSpan? ttl = null, params string[] tags)
    {
        return new CacheItem<T>
        {
            Key = key,
            Data = data,
            ExpiresAt = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : null,
            Tags = tags.ToList()
        };
    }

    /// <summary>
    /// 更新存取資訊
    /// </summary>
    public void UpdateAccess()
    {
        AccessCount++;
        LastAccessed = DateTime.UtcNow;
    }

    /// <summary>
    /// 延長過期時間
    /// </summary>
    /// <param name="additionalTime">額外時間</param>
    public void ExtendExpiry(TimeSpan additionalTime)
    {
        if (ExpiresAt.HasValue)
        {
            ExpiresAt = ExpiresAt.Value.Add(additionalTime);
        }
        else
        {
            ExpiresAt = DateTime.UtcNow.Add(additionalTime);
        }
    }

    /// <summary>
    /// 增加標籤
    /// </summary>
    /// <param name="tag">標籤</param>
    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
        }
    }

    /// <summary>
    /// 移除標籤
    /// </summary>
    /// <param name="tag">標籤</param>
    public void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }

    /// <summary>
    /// 檢查是否包含指定標籤
    /// </summary>
    /// <param name="tag">標籤</param>
    /// <returns>是否包含</returns>
    public bool HasTag(string tag)
    {
        return Tags.Contains(tag);
    }
}

/// <summary>
/// 非泛型快取項目 - 向後相容
/// </summary>
public class CacheItem : CacheItem<object>
{
    /// <summary>
    /// 建立非泛型快取項目
    /// </summary>
    /// <param name="key">快取鍵值</param>
    /// <param name="data">快取資料</param>
    /// <param name="ttl">存活時間</param>
    /// <param name="tags">標籤</param>
    /// <returns>快取項目</returns>
    public new static CacheItem Create(string key, object data, TimeSpan? ttl = null, params string[] tags)
    {
        return new CacheItem
        {
            Key = key,
            Data = data,
            ExpiresAt = ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : null,
            Tags = tags.ToList()
        };
    }
}