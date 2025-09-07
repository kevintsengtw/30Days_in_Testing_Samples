using System.Text.Json;

namespace TUnit.Advanced.DataDriven.Tests;

/// <summary>
/// 基本 MethodDataSource 範例測試
/// </summary>
public class MethodDataSourceTests
{
    /// <summary>
    /// 使用 MethodDataSource 測試百分比折扣計算
    /// </summary>
    [Test]
    [MethodDataSource(nameof(GetDiscountTestData))]
    public async Task CalculatePercentageDiscount_使用不同參數_應正確計算折扣金額(
        decimal orderAmount,
        decimal discountPercent,
        decimal expected)
    {
        // Arrange
        var calculator = new DiscountCalculator(new MockDiscountRepository(), new MockLogger<DiscountCalculator>());
        var order = new Order
        {
            OrderId = "TEST001",
            Items = [new OrderItem { UnitPrice = orderAmount, Quantity = 1 }]
        };
        var discountCode = $"PERCENT{discountPercent:F0}";

        // Act
        var actual = await calculator.CalculateDiscountAsync(order, discountCode);

        // Assert
        await Assert.That(actual).IsEqualTo(expected);
    }

    /// <summary>
    /// 提供折扣計算的測試資料
    /// </summary>
    public static IEnumerable<(decimal orderAmount, decimal discountPercent, decimal expected)> GetDiscountTestData()
    {
        yield return (1000m, 10m, 100m);
        yield return (2000m, 15m, 300m);
        yield return (500m, 20m, 100m);
        yield return (0m, 10m, 0m);
        yield return (1000m, 0m, 0m);
    }

    /// <summary>
    /// 使用 MethodDataSource 測試運費計算
    /// </summary>
    [Test]
    [MethodDataSource(nameof(GetShippingTestData))]
    public async Task CalculateShipping_根據客戶等級和金額_應正確計算運費(
        CustomerLevel level,
        decimal orderAmount,
        decimal expectedShipping)
    {
        // Arrange
        var calculator = new ShippingCalculator();
        var order = new Order
        {
            CustomerLevel = level,
            Items = [new OrderItem { UnitPrice = orderAmount, Quantity = 1 }]
        };

        // Act
        var actualShipping = calculator.CalculateShippingFee(order);

        // Assert
        await Assert.That(actualShipping).IsEqualTo(expectedShipping);
    }

    /// <summary>
    /// 提供客戶等級與運費的測試資料
    /// </summary>
    public static IEnumerable<(CustomerLevel level, decimal orderAmount, decimal expectedShipping)> GetShippingTestData()
    {
        // 一般會員
        yield return (CustomerLevel.一般會員, 500m, 80m);
        yield return (CustomerLevel.一般會員, 1000m, 0m); // 免運費

        // VIP 會員
        yield return (CustomerLevel.VIP會員, 500m, 40m); // 運費半價
        yield return (CustomerLevel.VIP會員, 1000m, 0m); // 免運費

        // 白金會員
        yield return (CustomerLevel.白金會員, 500m, 40m); // 運費半價
        yield return (CustomerLevel.白金會員, 999m, 40m); // 運費半價

        // 鑽石會員
        yield return (CustomerLevel.鑽石會員, 100m, 0m); // 永遠免運費
        yield return (CustomerLevel.鑽石會員, 500m, 0m); // 永遠免運費
    }

    /// <summary>
    /// 從檔案載入折扣測試資料
    /// </summary>
    [Test]
    [MethodDataSource(nameof(GetDiscountTestDataFromFile))]
    public async Task CalculateDiscount_從檔案讀取_應套用正確折扣(
        string scenario,
        decimal originalAmount,
        CustomerLevel level,
        string discountCode,
        decimal expectedDiscount)
    {
        // Arrange
        var calculator = new DiscountCalculator(new MockDiscountRepository(), new MockLogger<DiscountCalculator>());
        var order = new Order
        {
            CustomerLevel = level,
            Items = [new OrderItem { UnitPrice = originalAmount, Quantity = 1 }]
        };

        // Act
        var discount = await calculator.CalculateDiscountAsync(order, discountCode);

        // Assert
        await Assert.That(discount).IsEqualTo(expectedDiscount);
    }

    /// <summary>
    /// 從 JSON 檔案載入折扣測試資料
    /// </summary>
    public static IEnumerable<object[]> GetDiscountTestDataFromFile()
    {
        var filePath = Path.Combine("TestData", "discount-scenarios.json");
        var jsonData = File.ReadAllText(filePath);
        var scenarios = JsonSerializer.Deserialize<List<DiscountScenario>>(jsonData);
        if (scenarios == null)
        {
            yield break;
        }

        foreach (var s in scenarios)
        {
            yield return [s.Scenario, s.Amount, (CustomerLevel)s.Level, s.Code, s.Expected];
        }
    }

    /// <summary>
    /// 折扣測試情境對應 JSON 結構
    /// </summary>
    internal class DiscountScenario
    {
        public string Scenario { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Level { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Expected { get; set; }
    }
}

/// <summary>
/// Mock 折扣資料庫 - 簡化版本用於示範
/// </summary>
internal class MockDiscountRepository : IDiscountRepository
{
    public Task<DiscountRule?> GetDiscountRuleByCodeAsync(string discountCode)
    {
        // 百分比折扣碼
        if (discountCode.StartsWith("PERCENT"))
        {
            var percentStr = discountCode[7..]; // 移除 "PERCENT" 前綴
            if (decimal.TryParse(percentStr, out var percent))
            {
                return Task.FromResult<DiscountRule?>(new DiscountRule
                {
                    Code = discountCode,
                    Type = DiscountType.百分比折扣,
                    Value = percent,
                    IsActive = true,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(30)
                });
            }
        }

        // VIP 會員專用折扣碼
        if (discountCode == "VIP50")
        {
            return Task.FromResult<DiscountRule?>(new DiscountRule
            {
                Code = discountCode,
                Type = DiscountType.固定金額折扣,
                Value = 50m,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30),
                MinimumAmount = 1000m
            });
        }

        // 白金會員專用折扣碼 SAVE20
        if (discountCode == "SAVE20")
        {
            return Task.FromResult<DiscountRule?>(new DiscountRule
            {
                Code = discountCode,
                Type = DiscountType.固定金額折扣,
                Value = 250m,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30),
                MinimumAmount = 1000m
            });
        }

        return Task.FromResult<DiscountRule?>(null);
    }

    public Task<List<DiscountRule>> GetActiveDiscountRulesAsync()
    {
        return Task.FromResult(new List<DiscountRule>());
    }
}

/// <summary>
/// Mock Logger - 簡化版本用於示範
/// </summary>
internal class MockLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return false;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }
}