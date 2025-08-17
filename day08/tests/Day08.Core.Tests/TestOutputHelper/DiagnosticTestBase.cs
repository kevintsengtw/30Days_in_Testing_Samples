using System.Text.Json;
using Xunit.Abstractions;

namespace Day08.Core.Tests.TestOutputHelper;

/// <summary>
/// class DiagnosticTestBase - 診斷測試基底類別
/// </summary>
public class DiagnosticTestBase
{
    protected readonly ITestOutputHelper Output;

    /// <summary>
    /// DiagnosticTestBase 建構子
    /// </summary>
    /// <param name="testOutputHelper">測試輸出協助器</param>
    protected DiagnosticTestBase(ITestOutputHelper testOutputHelper)
    {
        this.Output = testOutputHelper;
    }

    /// <summary>
    /// 記錄測試上下文
    /// </summary>
    /// <param name="testName">測試名稱</param>
    /// <param name="testData">測試資料</param>
    protected void LogTestContext(string testName, object? testData = null)
    {
        this.Output.WriteLine($"=== {testName} ===");
        this.Output.WriteLine($"執行時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

        if (testData != null)
        {
            this.Output.WriteLine($"測試資料: {JsonSerializer.Serialize(testData, new JsonSerializerOptions { WriteIndented = true })}");
        }

        this.Output.WriteLine("");
    }

    /// <summary>
    /// 記錄例外資訊
    /// </summary>
    /// <param name="ex">例外物件</param>
    /// <param name="context">上下文資訊</param>
    protected void LogException(Exception ex, string context = "")
    {
        this.Output.WriteLine($"=== 例外發生 {context} ===");
        this.Output.WriteLine($"例外類型: {ex.GetType().Name}");
        this.Output.WriteLine($"例外訊息: {ex.Message}");
        this.Output.WriteLine($"堆疊追蹤:\n{ex.StackTrace}");
        this.Output.WriteLine("");
    }

    /// <summary>
    /// 記錄斷言失敗
    /// </summary>
    /// <param name="expected">預期值</param>
    /// <param name="actual">實際值</param>
    /// <param name="fieldName">欄位名稱</param>
    protected void LogAssertionFailure(string expected, string actual, string fieldName)
    {
        this.Output.WriteLine($"=== 斷言失敗 ===");
        this.Output.WriteLine($"欄位: {fieldName}");
        this.Output.WriteLine($"預期值: {expected}");
        this.Output.WriteLine($"實際值: {actual}");
        this.Output.WriteLine("");
    }
}