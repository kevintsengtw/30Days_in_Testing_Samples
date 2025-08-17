using Xunit.Abstractions;

namespace Day08.Core.Tests.Integration;

/// <summary>
/// 泛型 xUnit Logger 實作
/// </summary>
public class XUnitLoggerGeneric : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _categoryName;
    private readonly LoggerExternalScopeProvider _scopeProvider;

    /// <summary>
    /// XUnitLoggerGeneric 建構子
    /// </summary>
    /// <param name="testOutputHelper">測試輸出協助器</param>
    /// <param name="categoryName">記錄器類別名稱</param>
    /// <param name="scopeProvider">範圍提供者</param>
    public XUnitLoggerGeneric(ITestOutputHelper testOutputHelper, string categoryName, LoggerExternalScopeProvider scopeProvider)
    {
        this._testOutputHelper = testOutputHelper;
        this._categoryName = categoryName;
        this._scopeProvider = scopeProvider;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return this._scopeProvider.Push(state);
    }

    /// <summary>
    /// 記錄日誌
    /// </summary>
    /// <typeparam name="TState">日誌狀態類型</typeparam>
    /// <param name="logLevel">日誌級別</param>
    /// <param name="eventId">事件識別碼</param>
    /// <param name="state">日誌狀態</param>
    /// <param name="exception">例外資訊</param>
    /// <param name="formatter">格式化函數</param>
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