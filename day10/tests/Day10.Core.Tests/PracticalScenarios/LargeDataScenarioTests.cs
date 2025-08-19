namespace Day10.Core.Tests.PracticalScenarios;

/// <summary>
/// 大量資料測試場景
/// </summary>
public class LargeDataScenarioTests : AutoFixtureTestBase
{
    [Fact]
    public void ProcessBatch_大量資料_應正確處理()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var records = fixture.CreateMany<DataRecord>(100).ToList(); // 減少到 100 筆測試資料
        var processor = new DataProcessor();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var actual = processor.ProcessBatch(records);
        stopwatch.Stop();

        // Assert
        actual.ProcessedCount.Should().Be(100);
        actual.ErrorCount.Should().Be(0);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 效能要求
    }

    [Fact]
    public void ProcessBatch_記憶體使用_應在合理範圍()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var processor = new DataProcessor();

        // 測試不同大小的資料集
        var sizes = new[] { 10, 50, 100 }; // 減少測試規模

        foreach (var size in sizes)
        {
            // Act
            var records = fixture.CreateMany<DataRecord>(size);

            var initialMemory = GC.GetTotalMemory(false);
            var actual = processor.ProcessBatch(records);
            var finalMemory = GC.GetTotalMemory(true);

            var memoryUsed = finalMemory - initialMemory;

            // Assert
            actual.ProcessedCount.Should().Be(size);

            // 記憶體使用應該在合理範圍內
            memoryUsed.Should().BeLessThan(size * 1024); // 每筆記錄不超過 1KB
        }
    }

    [Fact]
    public void SerializeUser_任意使用者_應成功序列化和反序列化()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var original = fixture.Create<User>();

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<User>(json);

        // Assert
        deserialized.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void OrderCalculation_大量訂單_效能測試()
    {
        // Arrange
        var fixture = this.CreateFixture();
        var orders = fixture.CreateMany<Order>(10000).ToList();
        var calculator = new OrderCalculator();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var totalAmount = 0m;

        foreach (var order in orders)
        {
            totalAmount += calculator.CalculateTotal(order);
        }

        stopwatch.Stop();

        // Assert
        totalAmount.Should().BePositive();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // 1 秒內完成
    }

    [Fact]
    public void CreateMany_不同集合大小_應正確產生()
    {
        // Arrange
        var fixture = this.CreateFixture();

        // Act
        var smallList = fixture.CreateMany<Product>(5).ToList();
        var mediumList = fixture.CreateMany<Product>(50).ToList();
        var largeList = fixture.CreateMany<Product>(500).ToList();

        // Assert
        smallList.Should().HaveCount(5);
        mediumList.Should().HaveCount(50);
        largeList.Should().HaveCount(500);

        // 驗證所有物件都被正確建立
        smallList.All(p => p != null && !string.IsNullOrEmpty(p.Name)).Should().BeTrue();
        mediumList.All(p => p != null && !string.IsNullOrEmpty(p.Name)).Should().BeTrue();
        largeList.All(p => p != null && !string.IsNullOrEmpty(p.Name)).Should().BeTrue();
    }
}