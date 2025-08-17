using System.Collections.Concurrent;

namespace Day08.Core.Tests.Logging;

/// <summary>
/// class ConcurrentTestLogger - 並行測試用 Logger
/// </summary>
/// <typeparam name="T">Logger 類型</typeparam>
public class ConcurrentTestLogger<T> : ILogger<T>
{
    private readonly ConcurrentBag<LogEntry> _logs = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return new NoOpDisposable();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    /// <summary>
    /// 記錄訊息
    /// </summary>
    /// <param name="logLevel">記錄層級</param>
    /// <param name="eventId">事件編號</param>
    /// <param name="state">狀態</param>
    /// <param name="exception">例外</param>
    /// <param name="formatter">格式化函數</param>
    /// <typeparam name="TState"></typeparam>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception? exception, Func<TState, Exception?, string> formatter)
    {
        this._logs.Add(new LogEntry
        {
            Level = logLevel,
            Message = formatter(state, exception),
            State = state as IEnumerable<KeyValuePair<string, object>>,
            Exception = exception
        });
    }

    /// <summary>
    /// 取得記錄
    /// </summary>
    /// <param name="level">記錄層級</param>
    /// <returns></returns>
    public IList<LogEntry> GetLogs(LogLevel? level = null)
    {
        var allLogs = this._logs.ToList();
        return level.HasValue ? allLogs.Where(l => l.Level == level).ToList() : allLogs;
    }
}