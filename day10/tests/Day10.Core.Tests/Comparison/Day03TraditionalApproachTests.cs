using Day10.Core.Enums;

namespace Day10.Core.Tests.Comparison;

/// <summary>
/// Day 03 傳統方式的範例
/// </summary>
public class Day03TraditionalApproachTests
{
    [Fact]
    public void ProcessOrder_Day3方式_需要詳細的資料準備()
    {
        // Arrange - 需要大量的手動資料準備
        var customer = new Customer
        {
            Id = 1,
            Name = "John",
            Email = "john@example.com",
            Age = 30,
            Type = CustomerType.Premium
        };

        var item1 = new OrderItem
        {
            ProductId = 101,
            ProductName = "產品A",
            UnitPrice = 100,
            Quantity = 2
        };

        var item2 = new OrderItem
        {
            ProductId = 102,
            ProductName = "產品B",
            UnitPrice = 50,
            Quantity = 1
        };

        var order = new Order
        {
            Id = 1001,
            Customer = customer,
            Items = new List<OrderItem> { item1, item2 },
            Status = OrderStatus.Completed,
            OrderDate = new DateTime(2024, 3, 15)
        };

        var processor = new OrderProcessor();

        // Act
        var actual = processor.Process(order);

        // Assert
        actual.Success.Should().BeTrue();
        actual.TotalAmount.Should().Be(250); // 100*2 + 50*1
    }

    public static IEnumerable<object[]> GetUserTestData()
    {
        yield return ["John", "john@example.com", 25, true];
        yield return ["", "john@example.com", 25, false];     // 名稱為空
        yield return ["John", "invalid-email", 25, false];    // 無效 Email
        yield return ["John", "john@example.com", 10, false]; // 年齡過小
    }

    [Theory]
    [MemberData(nameof(GetUserTestData))]
    public void ValidateUser_Day3方式_使用預定義資料(
        string name, string email, int age, bool expected)
    {
        // Arrange
        var user = new User { Name = name, Email = email, Age = age };
        var validator = new UserValidator();

        // Act
        var actual = validator.IsValid(user);

        // Assert
        actual.Should().Be(expected);
    }
}