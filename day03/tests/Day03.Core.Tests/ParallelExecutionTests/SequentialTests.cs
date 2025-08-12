namespace Day03.Core.Tests.ParallelExecutionTests;

/// <summary>
/// class SequentialCollection - 定義一個測試集合，所有測試將依序執行。
/// </summary>
[CollectionDefinition("Sequential Tests", DisableParallelization = true)]
public class SequentialCollection : ICollectionFixture<object>
{
    // 此 Collection 中的所有測試將完全依序執行
}

/// <summary>
/// class IntegrationTests - 用於測試整合相關的功能。
/// </summary>
[Collection("Sequential Tests")]
public class IntegrationTests
{
    [Fact]
    public void Test_Step1()
    {
        // 這些測試會完全依序執行
        Thread.Sleep(100);
        Assert.True(true);
    }

    [Fact]
    public void Test_Step2()
    {
        Thread.Sleep(100);
        Assert.True(true);
    }
}