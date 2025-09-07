namespace TUnit.Advanced.Lifecycle.Tests;

/// <summary>
/// 重點測試：建構式與 [Before(Test)] 的執行順序
/// </summary>
public class ConstructorVsBeforeTestOrder
{
    private readonly List<string> _executionOrder = [];

    public ConstructorVsBeforeTestOrder()
    {
        _executionOrder.Add("建構式執行");
        Console.WriteLine("1. 建構式執行");
    }

    [Before(Test)]
    public async Task BeforeTest()
    {
        _executionOrder.Add("BeforeTest 執行");
        Console.WriteLine("2. BeforeTest 執行");
        await Task.Delay(1);
    }

    [Test]
    public async Task Test1_驗證執行順序()
    {
        Console.WriteLine("3. Test1 執行 - 驗證執行順序");
        _executionOrder.Add("Test1 執行");

        // 驗證執行順序
        await Assert.That(_executionOrder).HasCount().EqualTo(3);
        await Assert.That(_executionOrder[0]).IsEqualTo("建構式執行");
        await Assert.That(_executionOrder[1]).IsEqualTo("BeforeTest 執行");
        await Assert.That(_executionOrder[2]).IsEqualTo("Test1 執行");

        Console.WriteLine($"執行順序驗證完成: {string.Join(" -> ", _executionOrder)}");
    }

    [Test]
    public async Task Test2_驗證每個測試都有新實例()
    {
        Console.WriteLine("3. Test2 執行 - 驗證實例獨立性");
        _executionOrder.Add("Test2 執行");

        // 每個測試都應該有新的實例，所以 _executionOrder 應該重新開始
        await Assert.That(_executionOrder).HasCount().EqualTo(3);
        await Assert.That(_executionOrder[0]).IsEqualTo("建構式執行");
        await Assert.That(_executionOrder[1]).IsEqualTo("BeforeTest 執行");
        await Assert.That(_executionOrder[2]).IsEqualTo("Test2 執行");

        Console.WriteLine($"實例獨立性驗證完成: {string.Join(" -> ", _executionOrder)}");
    }

    [After(Test)]
    public async Task AfterTest()
    {
        Console.WriteLine("4. AfterTest 執行");
        await Task.Delay(1);
    }
}