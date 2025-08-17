namespace Day08.Core.Tests.Logging;

/// <summary>
/// 記錄項目
/// </summary>
public class LogEntry
{
    /// <summary>
    /// 記錄層級
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// 記錄訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 記錄狀態
    /// </summary>
    public IEnumerable<KeyValuePair<string, object>>? State { get; set; }

    /// <summary>
    /// 例外資訊
    /// </summary>
    public Exception? Exception { get; set; }
}