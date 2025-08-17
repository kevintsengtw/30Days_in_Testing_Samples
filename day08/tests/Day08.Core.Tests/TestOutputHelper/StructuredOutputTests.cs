using Day08.Core.Models;
using Xunit.Abstractions;

namespace Day08.Core.Tests.TestOutputHelper;

/// <summary>
/// class StructuredOutputTests - 結構化輸出格式測試範例
/// </summary>
public class StructuredOutputTests
{
    private readonly ITestOutputHelper _output;

    public StructuredOutputTests(ITestOutputHelper testOutputHelper)
    {
        this._output = testOutputHelper;
    }

    [Fact]
    public void ProcessOrder_包含多項商品_應計算正確總額()
    {
        // Arrange
        this.LogSection("=== 測試設置 ===");
        var order = new Order
        {
            Items =
            [
                new OrderItem { ProductName = "筆記型電腦", Price = 30000, Quantity = 1 },
                new OrderItem { ProductName = "滑鼠", Price = 800, Quantity = 2 },
                new OrderItem { ProductName = "鍵盤", Price = 1500, Quantity = 1 }
            ]
        };

        this.LogOrderDetails(order);

        // Act
        this.LogSection("=== 執行測試 ===");
        var startTime = DateTime.Now;
        var totalAmount = order.Items.Sum(item => item.Price * item.Quantity);
        var endTime = DateTime.Now;

        this.LogPerformance(startTime, endTime);

        // Assert
        this.LogSection("=== 驗證結果 ===");
        this._output.WriteLine($"計算總額: {totalAmount:C}");
        this._output.WriteLine($"預期總額: {33100:C}");

        totalAmount.Should().Be(33100); // 30000 + 800*2 + 1500 = 33100
        this.LogSection("=== 測試完成 ===");
    }

    /// <summary>
    /// 記錄區段標題
    /// </summary>
    /// <param name="title">區段標題</param>
    private void LogSection(string title)
    {
        this._output.WriteLine(title);
    }

    /// <summary>
    /// 記錄訂單明細
    /// </summary>
    /// <param name="order">訂單</param>
    private void LogOrderDetails(Order order)
    {
        this._output.WriteLine("訂單明細:");
        foreach (var item in order.Items)
        {
            this._output.WriteLine($"  - {item.ProductName}: {item.Price:C} x {item.Quantity}");
        }
    }

    /// <summary>
    /// 記錄效能資訊
    /// </summary>
    /// <param name="start">開始時間</param>
    /// <param name="end">結束時間</param>
    private void LogPerformance(DateTime start, DateTime end)
    {
        var duration = end - start;
        this._output.WriteLine($"執行時間: {duration.TotalMilliseconds:F2} ms");
    }
}