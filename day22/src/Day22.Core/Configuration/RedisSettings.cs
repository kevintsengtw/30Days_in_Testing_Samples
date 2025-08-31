namespace Day22.Core.Configuration;

/// <summary>
/// Redis 連線設定
/// </summary>
public class RedisSettings
{
    /// <summary>
    /// 連線字串
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// 資料庫編號
    /// </summary>
    public int Database { get; set; } = 0;

    /// <summary>
    /// 連線逾時時間（毫秒）
    /// </summary>
    public int ConnectTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// 命令逾時時間（毫秒）
    /// </summary>
    public int CommandTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// 重試次數
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// 重試延遲時間（毫秒）
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// 是否啟用 SSL
    /// </summary>
    public bool EnableSsl { get; set; } = false;

    /// <summary>
    /// 密碼
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 預設過期時間（分鐘）
    /// </summary>
    public int DefaultExpiryMinutes { get; set; } = 60;

    /// <summary>
    /// 鍵值前綴
    /// </summary>
    public string KeyPrefix { get; set; } = "app:";
}