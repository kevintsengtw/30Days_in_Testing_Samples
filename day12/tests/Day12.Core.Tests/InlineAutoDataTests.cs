namespace Day12.Core.Tests;

/// <summary>
/// InlineAutoData 混合固定值與自動產生測試
/// </summary>
public class InlineAutoDataTests
{
    [Theory]
    [InlineAutoData("VIP客戶", 1000)]
    [InlineAutoData("一般客戶", 500)]
    [InlineAutoData("新客戶", 100)]
    public void InlineAutoData_混合固定值與自動產生(string customerType, decimal creditLimit, Person person)
    {
        // Arrange
        // customerType 和 creditLimit 使用固定值
        // person 由 AutoFixture 自動產生

        // Act
        var customer = new Customer
        {
            Person = person,
            Type = customerType,
            CreditLimit = creditLimit
        };

        // Assert
        customer.Person.Should().NotBeNull();
        customer.Type.Should().Be(customerType);
        customer.CreditLimit.Should().Be(creditLimit);

        // 所有測試資料的信用額度都應該大於等於 100（最小值）
        customer.CreditLimit.Should().BeGreaterThanOrEqualTo(100);
    }

    [Theory]
    [InlineAutoData("產品A", 100)] // 依序對應 name, price
    [InlineAutoData("產品B", 200)]
    public void InlineAutoData_參數順序一致性(
        string name,     // 對應第1個固定值
        decimal price,   // 對應第2個固定值
        Product product) // 由 AutoFixture 產生
    {
        // Arrange & Act - 參數已設定

        // Assert
        name.Should().StartWith("產品");
        price.Should().BePositive();
        product.Should().NotBeNull();
    }

    [Theory]
    [InlineAutoData("電子產品")]
    [InlineAutoData("服飾用品")]
    [InlineAutoData("生活用品")]
    public void InlineAutoData_與DataAnnotation協作(
        string category,
        [Range(1, 1000)] decimal price,
        [StringLength(50)] string description,
        Product product)
    {
        // Arrange
        product.Name = $"{category}-{product.Name}";
        product.Price = price;
        product.Description = description;

        // Act
        var isValid = ValidateProduct(product);

        // Assert
        isValid.Should().BeTrue();
        product.Name.Should().StartWith(category);
        product.Price.Should().BeInRange(1, 1000);
        product.Description.Length.Should().BeLessThanOrEqualTo(50);
    }

    private static bool ValidateProduct(Product product)
    {
        return !string.IsNullOrEmpty(product.Name)
               && product.Price > 0
               && !string.IsNullOrEmpty(product.Description);
    }
}