using Xunit.Abstractions;

namespace Day08.Core.Tests.Integration;

/// <summary>
/// xUnit Logger 提供者，用於整合測試
/// </summary>
public class XUnitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly LoggerExternalScopeProvider _scopeProvider = new();

    /// <summary>
    /// XUnitLoggerProvider 建構子
    /// </summary>
    /// <param name="testOutputHelper">測試輸出協助器</param>
    public XUnitLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        this._testOutputHelper = testOutputHelper;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XUnitLoggerGeneric(this._testOutputHelper, categoryName, this._scopeProvider);
    }

    public void Dispose()
    {
    }
}