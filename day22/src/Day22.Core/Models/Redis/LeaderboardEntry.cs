using System.Text.Json.Serialization;

namespace Day22.Core.Models.Redis;

/// <summary>
/// 排行榜項目 - Redis Sorted Set 範例
/// </summary>
public class LeaderboardEntry
{
    /// <summary>
    /// 使用者 Id
    /// </summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 分數
    /// </summary>
    [JsonPropertyName("score")]
    public double Score { get; set; }

    /// <summary>
    /// 排名 (1 為最高)
    /// </summary>
    [JsonPropertyName("rank")]
    public long Rank { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 其他元資料
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();
}