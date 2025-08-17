using Xunit.Abstractions;

namespace Day08.Core.Tests.Logging;

/// <summary>
/// xUnit 測試用的 Logger 實作，將記錄訊息導向測試輸出
/// </summary>
/// <typeparam name="T">Logger 類型</typeparam>
public class XUnitLogger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _categoryName;
    private readonly LoggerExternalScopeProvider _scopeProvider;

    /// <summary>
    /// XUnitLogger 建構子
    /// </summary>
    /// <param name="testOutputHelper">測試輸出協助器</param>
    /// <param name="scopeProvider">範圍提供者</param>
    public XUnitLogger(ITestOutputHelper testOutputHelper, LoggerExternalScopeProvider scopeProvider)
    {
        this._testOutputHelper = testOutputHelper;
        this._categoryName = typeof(T).Name;
        this._scopeProvider = scopeProvider;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return this._scopeProvider.Push(state);
    }

    /// <summary>
    /// 開始記錄範圍
    /// </summary>
    /// <typeparam name="TState">狀態類型</typeparam>
    /// <param name="logLevel">記錄層級</param>
    /// <param name="eventId">事件編號</param>
    /// <param name="state">狀態</param>
    /// <param name="exception">例外</param>
    /// <param name="formatter">格式化函數</param>
    /// <typeparam name="TState"></typeparam>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        var logLine = $"[{DateTime.Now:HH:mm:ss.fff}] [{logLevel}] [{this._categoryName}] {message}";

        if (exception != null)
        {
            logLine += $"\n{exception}";
        }

        this._testOutputHelper.WriteLine(logLine);
    }
}