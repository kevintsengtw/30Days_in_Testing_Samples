namespace TUnit.Advanced.Lifecycle.Tests;

/// <summary>
/// 展示 TUnit 生命週期管理功能
/// </summary>
public class LifecycleTests
{
    private readonly StringBuilder _logBuilder;
    private static readonly List<string> ClassLog = [];

    public LifecycleTests()
    {
        Console.WriteLine("1. 建構式執行 - 測試實例建立");
        _logBuilder = new StringBuilder();
        _logBuilder.AppendLine("建構式執行");
    }

    [Before(Class)]
    public static async Task BeforeClass()
    {
        Console.WriteLine("2. BeforeClass 執行 - 類別層級初始化");
        ClassLog.Add("BeforeClass 執行");
        await Task.Delay(10); // 模擬非同步初始化
    }

    [Before(Test)]
    public async Task BeforeTest()
    {
        Console.WriteLine("3. BeforeTest 執行 - 測試前置設定");
        _logBuilder.AppendLine("BeforeTest 執行");
        await Task.Delay(5); // 模擬非同步設定
    }

    [Test]
    public async Task FirstTest_應按正確順序執行生命週期方法()
    {
        Console.WriteLine($"4. FirstTest 執行 - 驗證生命週期順序 [{DateTime.Now:HH:mm:ss.fff}]");
        _logBuilder.AppendLine("FirstTest 執行");

        var log = _logBuilder.ToString();
        await Assert.That(log).Contains("建構式執行");
        await Assert.That(log).Contains("BeforeTest 執行");
        await Assert.That(ClassLog).Contains("BeforeClass 執行");
    }

    [Test]
    public async Task SecondTest_應有獨立的實例()
    {
        Console.WriteLine($"4. SecondTest 執行 - 驗證實例獨立性 [{DateTime.Now:HH:mm:ss.fff}]");
        _logBuilder.AppendLine("SecondTest 執行");

        // 每個測試都有新的實例，所以建構式會重新執行
        var log = _logBuilder.ToString();
        await Assert.That(log).Contains("建構式執行");
        await Assert.That(log).Contains("BeforeTest 執行");
    }

    [After(Test)]
    public async Task AfterTest()
    {
        Console.WriteLine("5. AfterTest 執行 - 測試後清理");
        _logBuilder.AppendLine("AfterTest 執行");
        await Task.Delay(5); // 模擬非同步清理
    }

    [After(Class)]
    public static async Task AfterClass()
    {
        Console.WriteLine("6. AfterClass 執行 - 類別層級清理");
        ClassLog.Add("AfterClass 執行");
        await Task.Delay(10); // 模擬非同步清理
    }
}