namespace Day22.Core.Infrastructure;

/// <summary>
/// Redis 組態設定
/// </summary>
public class RedisConfig
{
    /// <summary>
    /// Redis 連線字串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 資料庫編號
    /// </summary>
    public int Database { get; set; } = 0;
}