namespace Day08.Core.Tests.Logging;

/// <summary>
/// class CompositeLogger - 組合 Logger，支援同時使用多個 Logger 實作
/// </summary>
/// <typeparam name="T">Logger 類型</typeparam>
public class CompositeLogger<T> : ILogger<T>
{
    private readonly ILogger<T>[] _loggers;

    /// <summary>
    /// CompositeLogger 建構子
    /// </summary>
    /// <param name="loggers">The logger.</param>
    public CompositeLogger(params ILogger<T>[] loggers)
    {
        _loggers = loggers;
    }

    /// <summary>
    /// 判斷指定的記錄層級是否啟用
    /// </summary>
    /// <param name="logLevel">記錄層級</param>
    /// <returns></returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return _loggers.Any(logger => logger.IsEnabled(logLevel));
    }

    /// <summary>
    /// 開始記錄範圍
    /// </summary>
    /// <typeparam name="TState">狀態類型</typeparam>
    /// <param name="state">狀態</param>
    /// <returns></returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scopes = _loggers.Select(logger => logger.BeginScope(state)).ToArray();
        return new CompositeDisposable(scopes);
    }

    /// <summary>
    /// 記錄訊息
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
        foreach (var logger in _loggers)
        {
            logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}

/// <summary>
/// class CompositeDisposable - 組合 Disposable 實作
/// </summary>
public class CompositeDisposable : IDisposable
{
    private readonly IDisposable?[] _disposables;

    /// <summary>
    /// CompositeDisposable 建構子
    /// </summary>
    /// <param name="disposables">要組合的 Disposable 實作</param>
    public CompositeDisposable(IDisposable?[] disposables)
    {
        _disposables = disposables;
    }

    /// <summary>
    /// 釋放資源
    /// </summary>
    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
    }
}