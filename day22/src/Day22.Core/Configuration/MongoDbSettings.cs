namespace Day22.Core.Configuration;

/// <summary>
/// MongoDB 連線設定
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// 連線字串
    /// </summary>
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    /// <summary>
    /// 資料庫名稱
    /// </summary>
    public string DatabaseName { get; set; } = "testdb";

    /// <summary>
    /// 使用者集合名稱
    /// </summary>
    public string UsersCollectionName { get; set; } = "users";

    /// <summary>
    /// 連線逾時時間（秒）
    /// </summary>
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 伺服器選擇逾時時間（秒）
    /// </summary>
    public int ServerSelectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Socket 逾時時間（秒）
    /// </summary>
    public int SocketTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 最大連線池大小
    /// </summary>
    public int MaxConnectionPoolSize { get; set; } = 100;

    /// <summary>
    /// 最小連線池大小
    /// </summary>
    public int MinConnectionPoolSize { get; set; } = 0;

    /// <summary>
    /// 是否啟用 SSL
    /// </summary>
    public bool EnableSsl { get; set; } = false;

    /// <summary>
    /// SSL 憑證驗證
    /// </summary>
    public bool SslVerifyCertificate { get; set; } = true;
}