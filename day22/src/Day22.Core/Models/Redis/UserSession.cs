using System.Text.Json.Serialization;

namespace Day22.Core.Models.Redis;

/// <summary>
/// 使用者會話資料 - Redis Hash 範例
/// </summary>
public class UserSession
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Session Id
    /// </summary>
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// IP 地址
    /// </summary>
    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// 使用者代理
    /// </summary>
    [JsonPropertyName("user_agent")]
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    [JsonPropertyName("last_activity")]
    public DateTime LastActivity { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 其他元資料
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// 檢查會話是否過期
    /// </summary>
    /// <param name="currentTime">當前時間</param>
    /// <param name="timeoutMinutes">過期時間（分鐘）</param>
    /// <returns>是否過期</returns>
    public bool IsExpired(DateTime currentTime, int timeoutMinutes = 30)
    {
        return currentTime.Subtract(LastActivity).TotalMinutes > timeoutMinutes;
    }

    /// <summary>
    /// 更新最後活動時間
    /// </summary>
    /// <param name="activityTime">活動時間</param>
    public void UpdateLastActivity(DateTime activityTime)
    {
        LastActivity = activityTime;
    }
}