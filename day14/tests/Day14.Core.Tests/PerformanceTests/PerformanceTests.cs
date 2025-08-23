namespace Day14.Core.Tests.PerformanceTests;

/// <summary>
/// 效能測試
/// </summary>
public class PerformanceTests
{
    private readonly ITestOutputHelper _output;

    public PerformanceTests(ITestOutputHelper output)
    {
        this._output = output;
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public void Bogus_GenerateProducts_PerformanceTest(int count)
    {
        // Arrange
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        var products = BogusDataGenerator.GenerateProducts(count);
        stopwatch.Stop();

        // Assert
        products.Should().HaveCount(count);
        this._output.WriteLine($"產生 {count} 個產品耗時: {stopwatch.ElapsedMilliseconds}ms");

        // 效能基準 (可依據實際硬體調整)
        if (count <= 1000)
        {
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
        }
        else if (count <= 10000)
        {
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000);
        }
    }

    [Theory]
    [InlineData(50)]
    [InlineData(500)]
    [InlineData(2000)]
    public void Bogus_GenerateOrders_PerformanceTest(int count)
    {
        // Arrange
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        var orders = BogusDataGenerator.OrderFaker.Generate(count);
        stopwatch.Stop();

        // Assert
        orders.Should().HaveCount(count);
        orders.Should().OnlyContain(o => o.Items.Any());

        this._output.WriteLine($"產生 {count} 個訂單耗時: {stopwatch.ElapsedMilliseconds}ms");
        this._output.WriteLine($"平均每個訂單: {(double)stopwatch.ElapsedMilliseconds / count:F2}ms");

        // 複雜物件的效能基準
        if (count <= 500)
        {
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000);
        }
    }

    [Fact]
    public void Bogus_vs_AutoFixture_Performance_Comparison()
    {
        // Arrange
        const int iterations = 1000;
        var bogusStopwatch = new Stopwatch();
        var autoFixtureStopwatch = new Stopwatch();

        // Act - Bogus 效能測試
        bogusStopwatch.Start();
        for (var i = 0; i < iterations; i++)
        {
            var product = BogusDataGenerator.ProductFaker.Generate();
        }

        bogusStopwatch.Stop();

        // Act - AutoFixture 效能測試
        autoFixtureStopwatch.Start();
        for (var i = 0; i < iterations; i++)
        {
            var product = AutoBogusDataGenerator.CreateProductWithAutoFixture();
        }

        autoFixtureStopwatch.Stop();

        // Assert & Report
        this._output.WriteLine($"Bogus 產生 {iterations} 次耗時: {bogusStopwatch.ElapsedMilliseconds}ms");
        this._output.WriteLine($"AutoFixture 產生 {iterations} 次耗時: {autoFixtureStopwatch.ElapsedMilliseconds}ms");
        this._output.WriteLine($"Bogus 平均: {(double)bogusStopwatch.ElapsedMilliseconds / iterations:F3}ms/次");
        this._output.WriteLine($"AutoFixture 平均: {(double)autoFixtureStopwatch.ElapsedMilliseconds / iterations:F3}ms/次");

        var performanceRatio = (double)autoFixtureStopwatch.ElapsedMilliseconds / bogusStopwatch.ElapsedMilliseconds;
        this._output.WriteLine($"效能比例 (AutoFixture/Bogus): {performanceRatio:F2}");

        // 基本效能要求
        bogusStopwatch.ElapsedMilliseconds.Should().BeLessThan(5000);
        autoFixtureStopwatch.ElapsedMilliseconds.Should().BeLessThan(10000);
    }

    [Fact]
    public void Memory_Usage_Test()
    {
        // Arrange
        const int objectCount = 5000;
        var initialMemory = GC.GetTotalMemory(true);

        // Act
        var products = BogusDataGenerator.GenerateProducts(objectCount);
        var afterCreationMemory = GC.GetTotalMemory(false);

        // 使用資料以確保不被最佳化
        var totalPrice = products.Sum(p => p.Price);

        var memoryUsed = afterCreationMemory - initialMemory;

        // Assert
        products.Should().HaveCount(objectCount);
        totalPrice.Should().BeGreaterThan(0);

        this._output.WriteLine($"產生 {objectCount} 個產品使用記憶體: {memoryUsed / 1024.0:F2} KB");
        this._output.WriteLine($"平均每個物件: {(double)memoryUsed / objectCount:F0} bytes");

        // 記憶體使用應該合理 (每個物件約數百 bytes)
        memoryUsed.Should().BeGreaterThan(0);
        memoryUsed.Should().BeLessThan(50 * 1024 * 1024); // 50MB 上限
    }

    [Fact]
    public void Thread_Safety_Test()
    {
        // Arrange
        const int threadCount = 10;
        const int operationsPerThread = 100;
        var results = new ConcurrentBag<Product>();
        var exceptions = new ConcurrentBag<Exception>();

        // Act
        var tasks = Enumerable.Range(0, threadCount).Select(_ => Task.Run(() =>
        {
            try
            {
                for (var i = 0; i < operationsPerThread; i++)
                {
                    var product = BogusDataGenerator.ProductFaker.Generate();
                    results.Add(product);
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        })).ToArray();

        Task.WaitAll(tasks);

        // Assert
        exceptions.Should().BeEmpty("Bogus 應該是執行緒安全的");
        results.Should().HaveCount(threadCount * operationsPerThread);
        results.Should().OnlyContain(p => !string.IsNullOrEmpty(p.Name));

        this._output.WriteLine($"多執行緒測試完成: {threadCount} 個執行緒各產生 {operationsPerThread} 個物件");
    }

    [Fact]
    public void Large_Dataset_Generation_Test()
    {
        // Arrange
        const int largeCount = 50000;
        var stopwatch = new Stopwatch();

        // Act
        stopwatch.Start();
        var users = BogusDataGenerator.UserFaker.Generate(largeCount);
        stopwatch.Stop();

        // Assert
        users.Should().HaveCount(largeCount);
        users.Should().OnlyContain(u => !string.IsNullOrEmpty(u.Email));

        // 檢查資料品質（允許一些重複，因為隨機產生可能產生重複）
        var uniqueEmails = users.Select(u => u.Email).Distinct().Count();
        var expectedMinUnique = (int)(largeCount * 0.95);
        uniqueEmails.Should().BeGreaterThanOrEqualTo(expectedMinUnique, "Email 唯一性應該達到 95% 以上");

        this._output.WriteLine($"產生 {largeCount} 個使用者耗時: {stopwatch.ElapsedMilliseconds}ms");
        this._output.WriteLine($"每秒產生: {largeCount * 1000.0 / stopwatch.ElapsedMilliseconds:F0} 個物件");

        // 大量資料產生效能要求
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(30000); // 30秒內完成
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Seeded_Faker_Consistency_Test(int seed)
    {
        // Arrange & Act
        var faker1 = new Faker<Product>().UseSeed(seed)
                                         .RuleFor(p => p.Id, f => f.Random.Int(1, 1000))
                                         .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                                         .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000));

        var faker2 = new Faker<Product>().UseSeed(seed)
                                         .RuleFor(p => p.Id, f => f.Random.Int(1, 1000))
                                         .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                                         .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000));

        var product1 = faker1.Generate();
        var product2 = faker2.Generate();

        // Assert
        product1.Id.Should().Be(product2.Id);
        product1.Name.Should().Be(product2.Name);
        product1.Price.Should().Be(product2.Price);

        this._output.WriteLine($"Seed {seed} 產生一致的結果: {product1.Name}");
    }
}