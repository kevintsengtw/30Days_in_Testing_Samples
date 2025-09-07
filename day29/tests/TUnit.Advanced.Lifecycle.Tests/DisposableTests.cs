namespace TUnit.Advanced.Lifecycle.Tests;

/// <summary>
/// 測試 TUnit 對 IDisposable 和 IAsyncDisposable 的支援
/// </summary>
public class DisposableTests : IDisposable, IAsyncDisposable
{
    private readonly List<string> _executionLog = [];

    public DisposableTests()
    {
        _executionLog.Add("建構式執行");
        Console.WriteLine($"1. 建構式執行 [{DateTime.Now:HH:mm:ss.fff}]");
    }

    [Before(Test)]
    public async Task BeforeTest()
    {
        _executionLog.Add("BeforeTest 執行");
        Console.WriteLine($"2. BeforeTest 執行 [{DateTime.Now:HH:mm:ss.fff}]");
        await Task.Delay(1);
    }

    [Test]
    public async Task Test1_驗證IDisposable支援()
    {
        _executionLog.Add("Test1 執行");
        Console.WriteLine($"3. Test1 執行 [{DateTime.Now:HH:mm:ss.fff}]");

        await Assert.That(_executionLog).Contains("建構式執行");
        await Assert.That(_executionLog).Contains("BeforeTest 執行");
        await Assert.That(_executionLog).Contains("Test1 執行");
    }

    [Test]
    public async Task Test2_驗證每個測試實例獨立()
    {
        _executionLog.Add("Test2 執行");
        Console.WriteLine($"3. Test2 執行 [{DateTime.Now:HH:mm:ss.fff}]");

        // 驗證每個測試都有新的實例
        await Assert.That(_executionLog).HasCount().EqualTo(3); // 建構式 + BeforeTest + Test2
    }

    [After(Test)]
    public async Task AfterTest()
    {
        _executionLog.Add("AfterTest 執行");
        Console.WriteLine($"4. AfterTest 執行 [{DateTime.Now:HH:mm:ss.fff}]");
        await Task.Delay(1);
    }

    // 同步 Dispose 方法
    public void Dispose()
    {
        _executionLog.Add("Dispose 執行");
        Console.WriteLine($"5. Dispose 執行 [{DateTime.Now:HH:mm:ss.fff}]");

        // 驗證執行順序
        Console.WriteLine($"最終執行順序: {string.Join(" -> ", _executionLog)}");
    }

    // 非同步 DisposeAsync 方法
    public async ValueTask DisposeAsync()
    {
        _executionLog.Add("DisposeAsync 執行");
        Console.WriteLine($"5. DisposeAsync 執行 [{DateTime.Now:HH:mm:ss.fff}]");

        await Task.Delay(1); // 模擬非同步清理工作

        // 驗證執行順序
        Console.WriteLine($"最終執行順序: {string.Join(" -> ", _executionLog)}");
    }
}

/// <summary>
/// 測試只實作 IDisposable 的情況
/// </summary>
public class SyncDisposableTests : IDisposable
{
    public SyncDisposableTests()
    {
        Console.WriteLine($"SyncDisposable 建構式 [{DateTime.Now:HH:mm:ss.fff}]");
    }

    [Test]
    public async Task SyncDisposableTest()
    {
        Console.WriteLine($"SyncDisposable 測試執行 [{DateTime.Now:HH:mm:ss.fff}]");
        await Assert.That(true).IsTrue();
    }

    public void Dispose()
    {
        Console.WriteLine($"SyncDisposable Dispose 執行 [{DateTime.Now:HH:mm:ss.fff}]");
    }
}

/// <summary>
/// 測試只實作 IAsyncDisposable 的情況
/// </summary>
public class AsyncDisposableTests : IAsyncDisposable
{
    public AsyncDisposableTests()
    {
        Console.WriteLine($"AsyncDisposable 建構式 [{DateTime.Now:HH:mm:ss.fff}]");
    }

    [Test]
    public async Task AsyncDisposableTest()
    {
        Console.WriteLine($"AsyncDisposable 測試執行 [{DateTime.Now:HH:mm:ss.fff}]");
        await Assert.That(true).IsTrue();
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine($"AsyncDisposable DisposeAsync 執行 [{DateTime.Now:HH:mm:ss.fff}]");
        await Task.Delay(10); // 模擬非同步清理
    }
}