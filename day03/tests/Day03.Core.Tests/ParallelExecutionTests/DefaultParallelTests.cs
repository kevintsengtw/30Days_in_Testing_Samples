namespace Day03.Core.Tests.ParallelExecutionTests;

// 預設情況：這兩個類別的測試可能會平行執行

/// <summary>
/// class UserServiceTests - 用於測試使用者服務的功能。
/// </summary>
public class UserServiceTests
{
    [Fact]
    public void Test1_可能與ProductServiceTests平行執行()
    {
        // 這個測試可能與 ProductServiceTests.Test2 平行執行
        Thread.Sleep(100); // 模擬一些工作
        Assert.True(true);
    }

    [Fact]
    public void Test1_1_同類別內依序執行()
    {
        // 同一類別內的測試會依序執行（不平行）
        Thread.Sleep(100);
        Assert.True(true);
    }
}

/// <summary>
/// class ProductServiceTests - 用於測試產品服務的功能。
/// </summary>
public class ProductServiceTests
{
    [Fact]
    public void Test2_可能與UserServiceTests平行執行()
    {
        // 這個測試可能與 UserServiceTests.Test1 平行執行
        Thread.Sleep(100); // 模擬一些工作
        Assert.True(true);
    }
}