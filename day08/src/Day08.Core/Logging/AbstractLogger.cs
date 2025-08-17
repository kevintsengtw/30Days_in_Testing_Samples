namespace Day08.Core.Logging;

/// <summary>
/// class AbstractLogger - 抽象 Logger 基底類別，簡化測試中的 Logger 驗證
/// </summary>
/// <typeparam name="T">Logger 類型</typeparam>
public abstract class AbstractLogger<T> : ILogger<T>
{
    /// <summary>
    /// 開始範圍（預設不實作）
    /// </summary>
    /// <typeparam name="TState">狀態類型</typeparam>
    /// <param name="state">狀態</param>
    /// <returns>可釋放的範圍</returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 檢查記錄層級是否啟用
    /// </summary>
    /// <param name="logLevel">記錄層級</param>
    /// <returns>是否啟用</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    /// <summary>
    /// 記錄訊息（泛型版本）
    /// </summary>
    /// <typeparam name="TState">狀態類型</typeparam>
    /// <param name="logLevel">記錄層級</param>
    /// <param name="eventId">事件編號</param>
    /// <param name="state">狀態</param>
    /// <param name="exception">例外</param>
    /// <param name="formatter">格式化函數</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception? exception, Func<TState, Exception?, string> formatter)
    {
        this.Log(logLevel, exception, formatter(state, exception));
    }

    /// <summary>
    /// 記錄訊息（簡化版本，供子類別實作）
    /// </summary>
    /// <param name="logLevel">記錄層級</param>
    /// <param name="ex">例外</param>
    /// <param name="information">訊息內容</param>
    public abstract void Log(LogLevel logLevel, Exception? ex, string information);
}