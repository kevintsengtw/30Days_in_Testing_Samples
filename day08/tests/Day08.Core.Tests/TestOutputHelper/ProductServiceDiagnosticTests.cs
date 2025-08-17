using Day08.Core.Models;
using Day08.Core.Services;
using Xunit.Abstractions;

namespace Day08.Core.Tests.TestOutputHelper;

/// <summary>
/// class ProductServiceDiagnosticTests - 商品服務診斷測試範例
/// </summary>
public class ProductServiceDiagnosticTests : DiagnosticTestBase
{
    /// <summary>
    /// ProductServiceDiagnosticTests 建構子
    /// </summary>
    /// <param name="testOutputHelper">測試輸出協助器</param>
    /// <returns></returns>
    public ProductServiceDiagnosticTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void CalculateTotalPrice_複雜折扣情境_應處理所有折扣計算()
    {
        try
        {
            // Arrange
            var testData = new
            {
                Customer = new { Type = "VIP", PurchaseHistory = 50000 },
                Items = new[]
                {
                    new { Name = "筆電", Price = 30000, Quantity = 1 },
                    new { Name = "滑鼠", Price = 1000, Quantity = 2 }
                },
                CouponCode = "SUMMER2024"
            };

            this.LogTestContext(nameof(this.CalculateTotalPrice_複雜折扣情境_應處理所有折扣計算), testData);

            var service = new ProductService();
            var customer = new Customer { Type = CustomerType.VIP, PurchaseHistory = 50000 };
            var items = new[]
            {
                new OrderItem { ProductName = "筆電", Price = 30000, Quantity = 1 },
                new OrderItem { ProductName = "滑鼠", Price = 1000, Quantity = 2 }
            };
            var couponCode = "SUMMER2024";

            // Act
            this.Output.WriteLine("開始執行價格計算...");
            var result = service.CalculateTotalPrice(customer, items, couponCode);
            this.Output.WriteLine($"計算結果: {result.TotalPrice:C}");

            // Assert
            var expectedPrice = 27200m; // 原價 32000 - VIP折扣 4800 - 優惠券折扣 3200 = 24000

            if (result.TotalPrice != expectedPrice)
            {
                this.LogAssertionFailure($"{expectedPrice:C}", $"{result.TotalPrice:C}", "TotalPrice");

                // 輸出詳細的計算過程
                this.Output.WriteLine("=== 計算明細 ===");
                this.Output.WriteLine($"原始金額: {result.OriginalAmount:C}");
                this.Output.WriteLine($"VIP 折扣: {result.VipDiscount:C}");
                this.Output.WriteLine($"優惠券折扣: {result.CouponDiscount:C}");
                this.Output.WriteLine($"最終金額: {result.TotalPrice:C}");
            }

            // 預期值：32000 - 4800 - 3200 = 24000
            result.TotalPrice.Should().Be(24000);
        }
        catch (Exception ex)
        {
            this.LogException(ex, "價格計算測試");
            throw;
        }
    }
}