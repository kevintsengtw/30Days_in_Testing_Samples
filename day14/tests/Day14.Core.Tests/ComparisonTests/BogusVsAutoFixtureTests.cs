namespace Day14.Core.Tests.ComparisonTests;

/// <summary>
/// Bogus 與 AutoFixture 比較測試
/// </summary>
public class BogusVsAutoFixtureTests
{
    [Fact]
    public void BogusVsAutoFixture_產生使用者資料_應展現不同特性()
    {
        // Arrange - Bogus 產生
        var bogusUser = BogusDataGenerator.UserFaker.Generate();

        // Arrange - AutoFixture 產生
        var autoFixtureUser = AutoBogusDataGenerator.CreateUserWithAutoFixture();

        // Act & Assert - Bogus 資料應該更真實
        bogusUser.Email.Should().Contain("@").And.NotContain("@example");
        bogusUser.Age.Should().BeInRange(18, 80); // 有邏輯限制
        bogusUser.Department.Should().BeOneOf("IT", "HR", "Finance", "Marketing");

        // AutoFixture 資料可能較不真實但一致
        autoFixtureUser.Should().NotBeNull();
        autoFixtureUser.FirstName.Should().NotBeNull();
        autoFixtureUser.LastName.Should().NotBeNull();
    }

    [Fact]
    public void BogusVsAutoFixture_產生產品資料_效能比較()
    {
        // Arrange
        const int count = 1000;
        var stopwatch = new Stopwatch();

        // Act - Bogus 效能測試
        stopwatch.Start();
        var bogusProducts = BogusDataGenerator.GenerateProducts(count);
        stopwatch.Stop();
        var bogusTime = stopwatch.ElapsedMilliseconds;

        // Act - AutoFixture 效能測試
        stopwatch.Restart();
        var autoFixtureProducts = AutoBogusDataGenerator.CreateMultipleProducts(count);
        stopwatch.Stop();
        var autoFixtureTime = stopwatch.ElapsedMilliseconds;

        // Assert - 驗證結果並記錄效能
        bogusProducts.Should().HaveCount(count);
        autoFixtureProducts.Should().HaveCount(count);

        // 輸出效能比較資訊
        Console.WriteLine($"Bogus 產生 {count} 個產品耗時: {bogusTime}ms");
        Console.WriteLine($"AutoFixture 產生 {count} 個產品耗時: {autoFixtureTime}ms");

        // 通常 Bogus 在大量產生時效能會根據複雜度而有所差異
        // 我們調整期望值為更寬鬆的範圍
        bogusTime.Should().BeLessThan(5000);       // 允許最多 5 秒
        autoFixtureTime.Should().BeLessThan(5000); // 允許最多 5 秒
    }

    [Fact]
    public void BogusVsAutoFixture_複雜物件產生_比較靈活性()
    {
        // Arrange & Act - Bogus 產生訂單
        var bogusOrder = BogusDataGenerator.OrderFaker.Generate();

        // Arrange & Act - AutoFixture 產生訂單
        var autoFixtureOrder = AutoBogusDataGenerator.CreateOrderWithAutoFixture();

        // Assert - Bogus 應該有更合理的業務邏輯
        // Bogus 產生的訂單
        bogusOrder.OrderNumber.Should().StartWith("ORD-");
        bogusOrder.Items.Should().NotBeEmpty();
        bogusOrder.TotalAmount.Should().Be(
            bogusOrder.SubTotal + bogusOrder.TaxAmount + bogusOrder.ShippingFee);

        // AutoFixture 產生的訂單
        autoFixtureOrder.Should().NotBeNull();
        autoFixtureOrder.OrderNumber.Should().NotBeNull();
        autoFixtureOrder.Items.Should().NotBeEmpty();

        // Bogus 的優勢：更真實的業務邏輯
        foreach (var item in bogusOrder.Items)
        {
            item.ProductName.Should().NotBeNull().And.NotBeEmpty();
            item.UnitPrice.Should().BeGreaterThan(0);
            item.Quantity.Should().BeGreaterThan(0);
            item.TotalPrice.Should().Be(item.UnitPrice * item.Quantity);
        }
    }

    [Fact]
    public void BogusVsAutoFixture_台灣本土化資料_比較準確性()
    {
        // Arrange & Act - Bogus 台灣人資料
        var bogusTaiwanPerson = BogusDataGenerator.TaiwanPersonFaker.Generate();

        // Arrange & Act - AutoFixture 台灣人資料
        var autoFixturePerson = AutoBogusDataGenerator.CreateTaiwanPersonWithAutoFixture();

        // Assert - Bogus 台灣本土化資料更準確
        bogusTaiwanPerson.City.Should().BeOneOf(TaiwanDataExtensions.Cities);

        bogusTaiwanPerson.University.Should().BeOneOf(TaiwanDataExtensions.Universities);

        bogusTaiwanPerson.Mobile.Should().StartWith("09").And.HaveLength(10);
        bogusTaiwanPerson.IdCard.Should().HaveLength(10);

        // AutoFixture 產生的資料可能不符合台灣格式
        autoFixturePerson.Should().NotBeNull();
        autoFixturePerson.Name.Should().NotBeNull();
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000)]
    public void BogusVsAutoFixture_記憶體使用比較(int count)
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);

        // Act - Bogus 記憶體測試
        var bogusData = BogusDataGenerator.GenerateProducts(count);
        var bogusMemory = GC.GetTotalMemory(false) - initialMemory;

        // 清理並重置
        bogusData = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var resetMemory = GC.GetTotalMemory(true);

        // Act - AutoFixture 記憶體測試
        var autoFixtureData = AutoBogusDataGenerator.CreateMultipleProducts(count);
        var autoFixtureMemory = GC.GetTotalMemory(false) - resetMemory;

        // Assert - 記錄記憶體使用情況
        Console.WriteLine($"產生 {count} 個產品:");
        Console.WriteLine($"Bogus 記憶體使用: {bogusMemory / 1024.0:F2} KB");
        Console.WriteLine($"AutoFixture 記憶體使用: {autoFixtureMemory / 1024.0:F2} KB");

        // 基本驗證
        bogusMemory.Should().BeGreaterThan(0);
        autoFixtureMemory.Should().BeGreaterThan(0);
    }

    [Fact]
    public void BogusVsAutoFixture_客製化程度比較()
    {
        // Arrange & Act - Bogus 自訂規則
        var bogusEmployee = BogusDataGenerator.EmployeeFaker.Generate();

        // Arrange & Act - AutoFixture 自訂規則
        var autoFixtureEmployee = AutoBogusDataGenerator.CreateEmployeeWithAutoFixture();

        // Assert - Bogus 客製化程度更高
        // 職級與年齡的合理性
        switch (bogusEmployee.Level)
        {
            case "Junior":
                bogusEmployee.Age.Should().BeLessThan(25);
                break;
            case "Senior":
                bogusEmployee.Age.Should().BeInRange(25, 34);
                break;
            case "Lead":
                bogusEmployee.Age.Should().BeInRange(35, 44);
                break;
            case "Principal":
                bogusEmployee.Age.Should().BeGreaterThanOrEqualTo(45);
                break;
        }

        // Email 格式一致性
        bogusEmployee.Email.Should().EndWith("@company.com");

        // 專案與技能的一致性
        foreach (var project in bogusEmployee.Projects)
        {
            project.Technologies.Should().BeSubsetOf(bogusEmployee.Skills);
        }

        // AutoFixture 基本檢驗
        autoFixtureEmployee.Should().NotBeNull();
        autoFixtureEmployee.Skills.Should().NotBeEmpty();
    }
}