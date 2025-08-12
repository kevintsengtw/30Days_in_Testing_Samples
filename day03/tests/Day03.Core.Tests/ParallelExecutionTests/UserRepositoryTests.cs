namespace Day03.Core.Tests.ParallelExecutionTests;

/// <summary>
/// class UserRepositoryTests - 用於測試使用者資料庫存取的功能。
/// </summary>
/// <remarks>
/// 使用相同 Collection 的測試不會平行執行
/// </remarks>
[Collection("Database Tests")]
public class UserRepositoryTests
{
    [Fact]
    public void Test1_不會與ProductRepositoryTests平行執行()
    {
        // 不會與 ProductRepositoryTests 的測試平行執行
        Thread.Sleep(100);
        Assert.True(true);
    }

    [Fact]
    public void Test1_1_同Collection內依序執行()
    {
        Thread.Sleep(100);
        Assert.True(true);
    }
}

/// <summary>
/// class ProductRepositoryTests - 用於測試產品資料庫存取的功能。
/// </summary>
[Collection("Database Tests")]
public class ProductRepositoryTests
{
    [Fact]
    public void Test2_不會與UserRepositoryTests平行執行()
    {
        // 不會與 UserRepositoryTests 的測試平行執行
        Thread.Sleep(100);
        Assert.True(true);
    }
}