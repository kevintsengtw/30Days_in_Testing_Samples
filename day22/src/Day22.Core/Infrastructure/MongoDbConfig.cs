namespace Day22.Core.Infrastructure;

/// <summary>
/// MongoDB 組態設定
/// </summary>
public class MongoDbConfig
{
    /// <summary>
    /// MongoDB 連線字串
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string DatabaseName { get; set; } = "testdb";
}