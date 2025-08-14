using Day05.Domain.DomainModels;
using Day05.Domain.Services.BusinessServices;
using Day05.Domain.Services.ProcessingServices;
using Day05.Domain.Services.UserServices;
using Day05.Tests.Extensions;

namespace Day05.Domain.Tests.PerformanceOptimizedTests;

// 提供更有意義的錯誤訊息
public class ErrorMessageOptimizationTests
{
    private readonly PaymentService _paymentService;
    private readonly UserService _userService;
    private readonly HeavyComputationService _heavyComputationService;

    public ErrorMessageOptimizationTests()
    {
        this._paymentService = new PaymentService();
        this._userService = new UserService();
        this._heavyComputationService = new HeavyComputationService();
    }

    [Fact]
    public void ProcessPayment_無效金額_應提供詳細錯誤訊息()
    {
        // Arrange
        var payment = new PaymentRequest { Amount = -100 };

        // Act
        var actual = this._paymentService.ProcessPayment(payment);

        // Assert
        // 提供詳細的錯誤上下文
        actual.IsSuccess.Should().BeFalse("因為付款金額不可為負數");
        actual.ErrorMessage.Should().Contain("金額", "因為錯誤訊息應明確指出有問題的欄位");
        actual.ErrorCode.Should().Be("INVALID_AMOUNT", "因為具體的錯誤代碼有助於問題排查");
    }

    [Fact]
    public void ComplexObjectValidation_詳細失敗分析_應提供清楚的失敗原因()
    {
        // Arrange
        var orderService = new OrderService();
        var invalidOrderData = new[]
        {
            new OrderItem { ProductId = 1, Quantity = 1, Price = 100.0m }
        };

        // Act
        var actual = orderService.CreateOrder(invalidOrderData);

        // Assert
        // 組合多個條件並提供清楚的失敗原因
        actual.Should().NotBeNull("因為訂單建立不應該回傳 null")
              .And.Subject.As<Order>().Items.Should().NotBeEmpty("因為訂單必須包含至少一個項目")
              .And.OnlyContain(item => item.Price > 0, "因為所有項目的價格都必須大於零");
    }
}

// 建立測試品質指標
public class TestQualityMetrics
{
    private readonly UserService _userService;
    private readonly HeavyComputationService _heavyComputationService;

    public TestQualityMetrics()
    {
        this._userService = new UserService();
        this._heavyComputationService = new HeavyComputationService();
    }

    [Fact]
    public void TestAssertion_應提供可操作的資訊()
    {
        // Arrange
        var testUserData = new UserProfile
        {
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var actual = this._userService.RegisterUser(testUserData);

        // Assert
        // 分層驗證，提供階段性失敗資訊
        actual.Should().NotBeNull("因為使用者註冊不應該完全失敗");
        actual.UserId.Should().BeGreaterThan(0, "因為應該分配有效的使用者ID");
        actual.Email.Should().Be("test@example.com", "因為電子郵件應該與輸入相符");
        actual.IsEmailVerified.Should().BeFalse("因為電子郵件初始時應該尚未驗證");
    }

    [Fact]
    public void PerformanceAssertion_應包含時間上下文()
    {
        // Arrange
        var largeDataSet = Enumerable.Range(1, 10000)
                                     .Select(i => new DataRecord { Id = i, Value = $"Record_{i}" })
                                     .ToList();

        var stopwatch = Stopwatch.StartNew();

        // Act
        var actual = this._heavyComputationService.ProcessLargeDataSet(largeDataSet);

        // Assert
        stopwatch.Stop();

        // 效能Assertions包含具體數據
        actual.Should().NotBeNull("因為處理應該要成功完成");
        stopwatch.Elapsed.Should().BeLessThan(
            TimeSpan.FromSeconds(5),
            $"因為處理 {largeDataSet.Count} 筆資料應在 5 秒內完成，但實際花費 {stopwatch.Elapsed.TotalSeconds:F2} 秒");
    }
}

// 建立團隊測試指南
public class TeamTestingGuidelines
{
    private readonly OrderService _orderService;
    private readonly UserService _userService;

    public TeamTestingGuidelines()
    {
        this._orderService = new OrderService();
        this._userService = new UserService();
    }

    // 標準 1：使用有意義的測試名稱
    [Fact]
    public void CreateUser_有效電子郵件_應回傳啟用的使用者()
    {
        // 遵循：被測試方法_測試情境_預期行為
        var actual = this._userService.CreateUser("test@example.com");
        actual.Should().NotBeNull();
        actual.Email.Should().Be("test@example.com");
    }

    // 標準 2：使用流暢的Assertions風格
    [Fact]
    public void ProcessOrder_多個項目_應計算正確的總金額()
    {
        // Arrange
        var validItems = new[]
        {
            new OrderItem { ProductId = 1, Quantity = 2, Price = 50.0m },
            new OrderItem { ProductId = 2, Quantity = 1, Price = 100.0m }
        };

        // Act
        var actual = this._orderService.CreateOrder(validItems);

        // Assert
        // 推薦：鏈式Assertions，邏輯清晰
        actual.Should().NotBeNull()
              .And.BeOfType<Order>();
        actual.TotalAmount.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCount(validItems.Length);
    }

    // 標準 3：適當使用自訂Assertions
    [Fact]
    public void RegisterUser_完整個人資料_應建立有效使用者()
    {
        // Arrange
        var completeProfile = new UserProfile
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Main St"
        };

        // Act
        var actual = this._userService.CreateUser("john.doe@example.com");
        actual.Profile = completeProfile;

        // Assert
        // 使用領域特定Assertions
        actual.ShouldBeValidUser();
        actual.Profile.ShouldBeComplete();
    }
}