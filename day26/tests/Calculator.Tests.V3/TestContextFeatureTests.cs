using System.Text;
using System.Text.Json;
using Calculator.Tests.V3.Fixtures;

namespace Calculator.Tests.V3;

/// <summary>
/// 展示 xUnit v3 TestContext 功能和改進
/// 基於官方範例展示 TestContext 的各種用途
/// </summary>
public class TestContextFeatureTests(ITestOutputHelper output, DatabaseAssemblyFixture dbFixture)
{
    private readonly Core.Calculator _calculator = new();

    /// <summary>
    /// 展示 TestContext 基本用法（概念性展示）
    /// </summary>
    [Fact]
    public void TestContext基本用法展示()
    {
        // Arrange
        var testName = nameof(TestContext基本用法展示);
        var startTime = DateTime.Now;

        output.WriteLine($"開始執行測試: {testName}");
        output.WriteLine($"測試開始時間: {startTime:yyyy-MM-dd HH:mm:ss.fff}");

        // 模擬 TestContext 的鍵值儲存功能
        var testData = new Dictionary<string, object>
        {
            ["startTime"] = startTime,
            ["testMethod"] = testName,
            ["calculator"] = _calculator
        };

        // Act
        var a = 5;
        var b = 3;
        var result = _calculator.Add(a, b);

        // 記錄測試執行過程
        var endTime = DateTime.Now;
        var duration = endTime - startTime;

        output.WriteLine($"計算過程: {a} + {b} = {result}");
        output.WriteLine($"執行時間: {duration.TotalMilliseconds:F2} 毫秒");

        // 模擬 TestContext 的檔案附加功能
        var resultData = $"測試結果報告\n測試方法: {testName}\n計算結果: {a} + {b} = {result}\n執行時間: {duration.TotalMilliseconds:F2} 毫秒";

        // 在真實的 TestContext 中，這會是：
        // TestContext.Current.AddAttachment("result.txt", "text/plain", Encoding.UTF8.GetBytes(resultData));
        output.WriteLine("模擬附加測試結果檔案");

        // Assert
        result.Should().Be(8);
        testData["startTime"].Should().NotBeNull();

        output.WriteLine("TestContext 基本用法測試完成");
    }

    /// <summary>
    /// 展示跨測試資料共享（透過 Assembly Fixture）
    /// </summary>
    [Fact]
    public async Task 展示跨測試資料共享()
    {
        // 透過 DatabaseAssemblyFixture 展示跨測試資料共享
        output.WriteLine("展示跨測試資料共享功能");

        // Arrange
        var calculator = dbFixture.GetService<Core.Calculator>();

        // Act
        var result = calculator.Multiply(6, 7);

        // 記錄到共享資料庫
        await dbFixture.LogCalculationAsync("multiply", 6, 7, result);

        // 查詢資料庫確認記錄
        var logs = await dbFixture.QueryAsync(
            "SELECT Operation, OperandA, OperandB, Result FROM CalculationLogs WHERE Operation = 'multiply'",
            reader => new
            {
                Operation = reader.GetString(0),
                OperandA = reader.GetDouble(1),
                OperandB = reader.GetDouble(2),
                Result = reader.GetDouble(3)
            });
        output.WriteLine($"資料庫中的計算記錄數量: {logs.Count}");
        output.WriteLine($"最新記錄: {logs.LastOrDefault()?.Operation} {logs.LastOrDefault()?.OperandA} × {logs.LastOrDefault()?.OperandB} = {logs.LastOrDefault()?.Result}");

        // Assert
        result.Should().Be(42);
        logs.Should().NotBeEmpty();
        logs.Last().Result.Should().Be(42);

        output.WriteLine("跨測試資料共享測試完成");
    }

    /// <summary>
    /// 展示測試附件功能
    /// </summary>
    [Fact]
    public void 展示測試附件功能()
    {
        output.WriteLine("展示測試附件功能");

        // Arrange
        var testData = new
        {
            TestName = nameof(展示測試附件功能),
            ExecutionTime = DateTime.Now,
            TestParameters = new { Input1 = 15, Input2 = 25 }
        };

        // Act
        var result = _calculator.Add(testData.TestParameters.Input1, testData.TestParameters.Input2);

        // 建立測試報告內容
        var report = new StringBuilder();
        report.AppendLine("=== 測試執行報告 ===");
        report.AppendLine($"測試名稱: {testData.TestName}");
        report.AppendLine($"執行時間: {testData.ExecutionTime:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"輸入參數: {testData.TestParameters.Input1}, {testData.TestParameters.Input2}");
        report.AppendLine($"計算結果: {result}");
        report.AppendLine($"測試狀態: {(result == 40 ? "通過" : "失敗")}");

        var reportContent = report.ToString();

        // 在真實的 TestContext 中，這會是：
        // TestContext.Current.AddAttachment("test-report.txt", "text/plain", Encoding.UTF8.GetBytes(reportContent));

        output.WriteLine("模擬附加測試報告檔案:");
        output.WriteLine(reportContent);

        // 模擬附加 JSON 格式的測試資料
        var jsonData = JsonSerializer.Serialize(testData, new JsonSerializerOptions { WriteIndented = true });
        output.WriteLine("模擬附加 JSON 資料檔案:");
        output.WriteLine(jsonData);

        // Assert
        result.Should().Be(40);

        output.WriteLine("測試附件功能展示完成");
    }
}