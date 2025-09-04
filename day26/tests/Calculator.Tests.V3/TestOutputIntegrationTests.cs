using System.Diagnostics;

namespace Calculator.Tests.V3;

/// <summary>
/// 展示 xUnit v3 中 TestOutputHelper 與 Console 的整合使用
/// 基於官方 TestOutputExample 範例
/// </summary>
public class TestOutputIntegrationTests(ITestOutputHelper output)
{
    /// <summary>
    /// 透過 ITestOutputHelper 輸出測試資訊
    /// </summary>
    [Fact]
    public void 透過TestOutputHelper輸出資訊()
    {
        // Arrange
        var calculator = new Core.Calculator();

        // Act
        output.WriteLine("開始執行計算測試...");
        var result = calculator.Add(5, 3);
        output.WriteLine($"計算結果: 5 + 3 = {result}");

        // Assert
        result.Should().Be(8);
        output.WriteLine("測試通過！");
    }

    /// <summary>
    /// 透過 TestContext 輸出資訊（如果支援）
    /// </summary>
    [Fact]
    public void 透過TestContext輸出資訊()
    {
        // xUnit v3 中的 TestContext 用法
        // 注意：實際 API 可能因版本而異
        var calculator = new Core.Calculator();

        // 模擬 TestContext 的使用
        var testName = nameof(透過TestContext輸出資訊);
        output.WriteLine($"測試方法: {testName}");

        // Act
        var result = calculator.Multiply(4, 6);
        output.WriteLine($"執行乘法運算: 4 × 6 = {result}");

        // Assert
        result.Should().Be(24);
    }

    /// <summary>
    /// 透過 Console 輸出（會被 xUnit v3 捕獲）
    /// </summary>
    [Fact]
    public void 透過Console輸出資訊()
    {
        // Arrange
        var calculator = new Core.Calculator();

        // 使用 Console.WriteLine，xUnit v3 會自動捕獲這些輸出
        Console.WriteLine("開始執行除法測試");
        Console.WriteLine($"測試時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        // Act
        var result = calculator.Divide(15, 3);
        Console.WriteLine($"計算過程: 15 ÷ 3 = {result}");

        // Assert
        result.Should().Be(5);
        Console.WriteLine("除法測試完成");
    }

    /// <summary>
    /// 透過 Trace 輸出診斷資訊
    /// </summary>
    [Fact]
    public void 透過Trace輸出診斷資訊()
    {
        // Arrange
        var calculator = new Core.Calculator();
        var numbers = new[] { 1, 2, 3, 4, 5 };

        Trace.WriteLine("開始執行陣列求和測試");
        Trace.WriteLine($"輸入陣列: [{string.Join(", ", numbers)}]");

        // Act
        var sum = numbers.Aggregate(0, (acc, num) => calculator.Add(acc, num));
        Trace.WriteLine($"累加結果: {sum}");

        // Assert
        sum.Should().Be(15);
        Trace.WriteLine("累加測試完成");
    }

    /// <summary>
    /// 組合使用多種輸出方式
    /// </summary>
    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(20, 4, 5)]
    [InlineData(100, 10, 10)]
    public void 組合使用多種輸出方式(int dividend, int divisor, int expected)
    {
        // 使用 TestOutputHelper
        output.WriteLine($"測試案例: {dividend} ÷ {divisor}");

        // 使用 Console（會被自動捕獲）
        Console.WriteLine($"預期結果: {expected}");

        // 使用 Trace 進行診斷
        Trace.WriteLine("[TRACE] 開始執行除法運算");

        // Arrange
        var calculator = new Core.Calculator();

        // Act
        var result = calculator.Divide(dividend, divisor);

        // 記錄實際結果
        output.WriteLine($"實際結果: {result}");
        Console.WriteLine($"測試 {dividend}÷{divisor}={result} 完成");
        Trace.WriteLine($"[TRACE] 除法運算完成，結果: {result}");

        // Assert
        result.Should().Be(expected);
    }

    /// <summary>
    /// 展示錯誤情況下的輸出
    /// </summary>
    [Fact]
    public void 展示錯誤處理的輸出()
    {
        // Arrange
        var calculator = new Core.Calculator();

        output.WriteLine("測試除以零的錯誤處理");
        Console.WriteLine("這個測試會故意觸發例外狀況");

        // Act & Assert
        var exception = Assert.Throws<DivideByZeroException>(() =>
        {
            Console.WriteLine("嘗試執行 10 ÷ 0...");
            calculator.Divide(10, 0);
        });

        output.WriteLine($"成功捕獲預期的例外: {exception.Message}");
        Console.WriteLine("錯誤處理測試通過");
        Trace.WriteLine("[TRACE] 例外處理測試完成");
    }

    /// <summary>
    /// 效能測試的輸出範例
    /// </summary>
    [Fact]
    public void 效能測試輸出範例()
    {
        // Arrange
        var calculator = new Core.Calculator();
        var iterations = 1000;

        output.WriteLine($"開始效能測試 ({iterations:N0} 次運算)");

        // Act
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < iterations; i++)
        {
            calculator.Add(i, i + 1);
        }

        stopwatch.Stop();

        // 輸出效能資訊
        var elapsed = stopwatch.ElapsedMilliseconds;
        var opsPerSecond = iterations / (elapsed / 1000.0);

        output.WriteLine($"執行時間: {elapsed} 毫秒");
        output.WriteLine($"每秒運算次數: {opsPerSecond:N0} ops/sec");
        Console.WriteLine($"平均每次運算: {elapsed / (double)iterations:F3} 毫秒");

        // Assert
        elapsed.Should().BeLessThan(1000, "運算應該在 1 秒內完成");

        output.WriteLine("效能測試通過");
    }
}