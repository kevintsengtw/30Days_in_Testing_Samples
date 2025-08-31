using System.Text.Json.Serialization;

namespace Day22.Core.Models.Redis;

/// <summary>
/// 通知訊息 - Redis Stream 範例
/// </summary>
public class NotificationMessage
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 使用者 Id
    /// </summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 標題
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 內容
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 類型 (例如: "info", "warning", "alert")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 優先級 (數字越大表示優先級越高)
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; } = 1;

    /// <summary>
    /// 建立時間
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 讀取時間，null 表示未讀
    /// </summary>
    [JsonPropertyName("read_at")]
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    [JsonPropertyName("is_read")]
    public bool IsRead => ReadAt.HasValue;

    /// <summary>
    /// 其他元資料
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// 標記為已讀
    /// </summary>
    /// <param name="readTime">讀取時間</param>
    public void MarkAsRead(DateTime readTime)
    {
        ReadAt = readTime;
    }
}