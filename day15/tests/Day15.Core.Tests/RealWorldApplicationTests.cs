using System;
using AwesomeAssertions;
using Day15.Core.Models;
using Day15.TestLibrary.TestData.Base;

namespace Day15.Core.Tests;

/// <summary>
/// 實際應用場景測試
/// </summary>
public class RealWorldApplicationTests : TestBase
{
    public RealWorldApplicationTests() : base(seed: 789)
    {
    }

    [Fact]
    public void CreateOrder_使用整合資料產生_應正確建立訂單()
    {
        // Arrange
        var orderService = new OrderService();
        var customer = Create<User>();
        var products = CreateMany<Product>(3);

        // Act
        var order = orderService.CreateOrder(customer, products);

        // Assert
        order.Should().NotBeNull();
        order.Customer.Should().Be(customer);
        order.Items.Should().HaveCount(3);
        order.TotalAmount.Should().BeGreaterThan(0);
        order.OrderDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void UserRegistration_使用真實感資料_應通過驗證()
    {
        // Arrange
        var userService = new UserService();
        var user = Generate<User>();

        // Act
        var validationResult = userService.ValidateUser(user);

        // Assert
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();

        // 驗證 Bogus 產生的資料格式正確
        user.Email.Should().MatchRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        user.FirstName.Should().NotBeNullOrWhiteSpace();
        user.LastName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CompanyAnalysis_使用完整測試場景_應正確統計()
    {
        // Arrange
        var analysisService = new CompanyAnalysisService();
        var scenario = Factory.CreateTestScenario();

        // Act
        var analysis = analysisService.AnalyzeCompany(scenario.Company, scenario.Orders);

        // Assert
        analysis.Should().NotBeNull();
        analysis.TotalEmployees.Should().Be(scenario.Company.Employees.Count);
        analysis.TotalOrders.Should().Be(scenario.Orders.Count);
        analysis.TotalRevenue.Should().Be(scenario.Orders.Sum(o => o.TotalAmount));
        analysis.AverageOrderValue.Should().BeGreaterThan(0);
    }

    [Fact]
    public void BulkDataProcessing_大量資料處理_應正確執行()
    {
        // Arrange
        var dataProcessor = new BulkDataProcessor();
        var users = Factory.CreateMany<User>(100);
        var orders = Factory.CreateMany<Order>(500);

        // Act
        var result = dataProcessor.ProcessUserOrders(users, orders);

        // Assert
        result.Should().NotBeNull();
        result.ProcessedUsers.Should().HaveCount(100);
        result.ProcessedOrders.Should().HaveCount(500);
        result.ProcessingErrors.Should().BeEmpty();
    }
}

#region 模擬的業務邏輯類別

/// <summary>
/// 訂單服務（模擬）
/// </summary>
public class OrderService
{
    public Order CreateOrder(User customer, List<Product> products)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Customer = customer,
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
            Items = products.Select(p => new OrderItem
            {
                Id = Guid.NewGuid(),
                Product = p,
                Quantity = Random.Shared.Next(1, 5),
                UnitPrice = p.Price
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(i => i.TotalPrice);
        return order;
    }
}

/// <summary>
/// 使用者服務（模擬）
/// </summary>
public class UserService
{
    public ValidationResult ValidateUser(User user)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(user.FirstName))
            errors.Add("FirstName is required");

        if (string.IsNullOrWhiteSpace(user.LastName))
            errors.Add("LastName is required");

        if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
            errors.Add("Valid email is required");

        return new ValidationResult
        {
            IsValid = !errors.Any(),
            Errors = errors
        };
    }
}

/// <summary>
/// 公司分析服務（模擬）
/// </summary>
public class CompanyAnalysisService
{
    public CompanyAnalysis AnalyzeCompany(Company company, List<Order> orders)
    {
        var companyOrders = orders.Where(o =>
            company.Employees.Any(e => e.Id == o.Customer.Id)).ToList();

        return new CompanyAnalysis
        {
            CompanyName = company.Name,
            TotalEmployees = company.Employees.Count,
            TotalOrders = companyOrders.Count,
            TotalRevenue = companyOrders.Sum(o => o.TotalAmount),
            AverageOrderValue = companyOrders.Any()
                ? companyOrders.Average(o => o.TotalAmount)
                : 0
        };
    }
}

/// <summary>
/// 大量資料處理器（模擬）
/// </summary>
public class BulkDataProcessor
{
    public BulkProcessingResult ProcessUserOrders(List<User> users, List<Order> orders)
    {
        var result = new BulkProcessingResult
        {
            ProcessedUsers = users,
            ProcessedOrders = orders,
            ProcessingErrors = new List<string>()
        };

        // 模擬處理邏輯
        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                result.ProcessingErrors.Add($"User {user.Id} has invalid email");
            }
        }

        return result;
    }
}

/// <summary>
/// 驗證結果
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// 公司分析結果
/// </summary>
public class CompanyAnalysis
{
    public string CompanyName { get; set; } = string.Empty;
    public int TotalEmployees { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}

/// <summary>
/// 大量處理結果
/// </summary>
public class BulkProcessingResult
{
    public List<User> ProcessedUsers { get; set; } = new();
    public List<Order> ProcessedOrders { get; set; } = new();
    public List<string> ProcessingErrors { get; set; } = new();
}

#endregion
