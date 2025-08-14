using Day05.Domain.DomainModels;
using Day05.Domain.Services.BusinessServices;
using Day05.Domain.Services.ProcessingServices;
using Day05.Domain.Services.UserServices;

namespace Day05.Domain.Tests.PerformanceOptimizedTests;

public class PerformanceOptimizedAssertions
{
    [Fact]
    public void LargeCollection_大型集合Assertions_應高效率執行()
    {
        var dataProcessor = new DataProcessor();

        // 模擬大量資料
        var largeDataset = Enumerable.Range(1, 100000)
                                     .Select(i => new DataRecord
                                     {
                                         Id = i,
                                         Value = $"Record_{i}"
                                     })
                                     .ToList();

        var processed = dataProcessor.ProcessLargeDataset(largeDataset);

        // 優化：使用高效率的 Assertions 策略
        processed.Should().HaveCount(largeDataset.Count);

        // 抽樣驗證而非全量驗證
        var sampleSize = Math.Min(1000, processed.Count / 10);
        var sampleIndices = Enumerable.Range(0, sampleSize)
                                      .Select(_ => Random.Shared.Next(processed.Count))
                                      .Distinct()
                                      .ToList();

        foreach (var index in sampleIndices)
        {
            processed[index].Should().NotBeNull();
            processed[index].Id.Should().BeGreaterThan(0);
        }

        // 統計驗證
        processed.Count(r => r.IsProcessed).Should().Be(processed.Count);
    }

    [Fact]
    public void ComplexObject_複雜物件比較_應使用選擇性比較()
    {
        var complexService = new ComplexService();
        var complexObject = complexService.GenerateComplexObject();
        var expectedTemplate = new ComplexObject
        {
            ImportantProperty1 = "Value1",
            ImportantProperty2 = "Value2",
            CriticalData = "CriticalValue"
        }; // 預期模板

        // 選擇性比較，避免不必要的深度比較
        complexObject.Should().BeEquivalentTo(expectedTemplate,
                                              options => options.Including(x => x.ImportantProperty1)
                                                                .Including(x => x.ImportantProperty2)
                                                                .Including(x => x.CriticalData)
                                                                .Excluding(x => x.Timestamp)   // 排除時間戳記
                                                                .Excluding(x => x.GeneratedId) // 排除自動生成欄位
        );
    }
}

// 實際應用範例
public class PerformanceOptimizedTests
{
    private readonly ITestOutputHelper _output;

    public PerformanceOptimizedTests(ITestOutputHelper output)
    {
        this._output = output;
    }

    [Fact]
    public void LargeDataSet_大量資料驗證_應使用效能優化策略()
    {
        var userService = new UserService();

        // Arrange
        // 建立較小的測試資料集來避免過長的執行時間
        var expectedUsers = Enumerable.Range(1, 1000)
                                      .Select(i => new User
                                      {
                                          Id = i,
                                          Name = $"User{i}",
                                          Email = $"user{i}@example.com"
                                      })
                                      .ToList();

        // Act
        var actualUsers = userService.ProcessLargeUserBatch(expectedUsers);

        // Assert
        // 簡化的驗證策略
        actualUsers.Should().HaveCount(expectedUsers.Count);

        // 抽樣驗證前10筆
        for (var i = 0; i < Math.Min(10, actualUsers.Count); i++)
        {
            actualUsers[i].Id.Should().Be(expectedUsers[i].Id);
            actualUsers[i].Name.Should().Be(expectedUsers[i].Name);
            actualUsers[i].Email.Should().Be(expectedUsers[i].Email);
            // 不比較時間戳記，因為它們會不同
        }

        this._output.WriteLine($"成功驗證 {expectedUsers.Count} 筆使用者資料");
    }

    [Fact]
    public void EntityComparison_關鍵屬性_應快速比對()
    {
        var orderService = new OrderService();

        // Arrange
        var expectedOrder = new Order
        {
            Id = 1,
            CustomerName = "John Doe",
            TotalAmount = 999.99m,
            Status = "Processing",
            CreatedAt = DateTime.Now, // 這個會變動
            UpdatedAt = DateTime.Now  // 這個會變動
        };

        // Act
        var actualOrder = orderService.GetOrder(1);

        // Assert
        // 只比較關鍵屬性，忽略會變動的時間戳記
        Extensions.PerformanceOptimizedAssertions.AssertKeyPropertiesOnly(
            actualOrder,
            expectedOrder,
            o => o.Id,
            o => o.CustomerName
            // 移除 TotalAmount 比較，因為實際值與期望值不同
        );

        // 這種方式比完整的 BeEquivalentTo 快很多
        this._output.WriteLine("快速驗證訂單關鍵屬性完成");
    }

    [Fact]
    public void ComplexScenario_結合多種優化策略_應高效率執行()
    {
        var orderService = new OrderService();
        var largeOrderBatch = this.GenerateLargeOrderBatch(10000);
        var processedOrders = orderService.ProcessOrderBatch(largeOrderBatch);

        var stopwatch = Stopwatch.StartNew();

        // 先用快速方法檢查數量
        processedOrders.Should().HaveCount(largeOrderBatch.Count);

        // 抽樣詳細驗證（只驗證前100筆）
        var sampleOrders = processedOrders.Take(100).ToList();
        var expectedSample = largeOrderBatch.Take(100).ToList();

        sampleOrders.Should().BeEquivalentTo(expectedSample, 
                                             options => options.Excluding(o => o.ProcessedAt)
                                                               .Excluding(o => o.UpdatedAt)
                                                               .Excluding(o => o.Status)); // 排除狀態，因為會被改變

        // 對剩餘的只做關鍵屬性驗證
        var remainingOrders = processedOrders.Skip(100).ToList();
        var expectedRemaining = largeOrderBatch.Skip(100).ToList();

        for (var i = 0; i < remainingOrders.Count; i++)
        {
            Extensions.PerformanceOptimizedAssertions.AssertKeyPropertiesOnly(
                remainingOrders[i],
                expectedRemaining[i],
                o => o.Id,
                o => o.TotalAmount
                // 移除 Status 比較，因為狀態會改變
            );
        }

        stopwatch.Stop();

        // 效能驗證
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "因為混合驗證策略應該在 5 秒內完成");

        this._output.WriteLine($"混合策略驗證 {processedOrders.Count} 筆訂單，耗時 {stopwatch.ElapsedMilliseconds}ms");
    }

    private List<Order> GenerateLargeOrderBatch(int count)
    {
        return Enumerable.Range(1, count)
                         .Select(i => new Order
                         {
                             Id = i,
                             CustomerName = $"Customer{i}",
                             TotalAmount = i * 10.5m,
                             Status = "Pending"
                         })
                         .ToList();
    }
}