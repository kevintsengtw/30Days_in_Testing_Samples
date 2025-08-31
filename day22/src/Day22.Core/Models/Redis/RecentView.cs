using System.Text.Json.Serialization;

namespace Day22.Core.Models.Redis;

/// <summary>
/// 最近檢視項目 - Redis List 範例
/// </summary>
public class RecentView
{
    /// <summary>
    /// Item Id
    /// </summary>
    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = string.Empty;

    /// <summary>
    /// Item 類型 (例如: "article", "video", "product")
    /// </summary>
    [JsonPropertyName("item_type")]
    public string ItemType { get; set; } = string.Empty;

    /// <summary>
    /// Item 標題
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 檢視時間
    /// </summary>
    [JsonPropertyName("viewed_at")]
    public DateTime ViewedAt { get; set; }

    /// <summary>
    /// 檢視持續時間
    /// </summary>
    [JsonPropertyName("view_duration")]
    public TimeSpan ViewDuration { get; set; }

    /// <summary>
    /// 其他元資料
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}