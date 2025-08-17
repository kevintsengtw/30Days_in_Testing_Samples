using Day08.Core.Models;
using Day08.Core.Services;
using Xunit.Abstractions;

namespace Day08.Core.Tests.TestOutputHelper;

/// <summary>
/// class ProductServiceTests - ITestOutputHelper 基礎使用範例
/// </summary>
public class ProductServiceTests
{
    private readonly ITestOutputHelper _output;

    public ProductServiceTests(ITestOutputHelper testOutputHelper)
    {
        this._output = testOutputHelper;
    }

    [Fact]
    public void CalculateDiscount_VIP客戶購買高價商品_應回傳20百分比折扣()
    {
        // Arrange
        var customer = new Customer { Type = CustomerType.VIP, PurchaseHistory = 15000 };
        var product = new Product { Price = 1000, Category = "Electronics" };

        this._output.WriteLine($"Testing VIP customer: {customer.Type}, History: {customer.PurchaseHistory}");
        this._output.WriteLine($"Product: {product.Category}, Price: {product.Price}");

        var service = new ProductService();

        // Act
        var discount = service.CalculateDiscount(customer, product);

        // Assert
        this._output.WriteLine($"Calculated discount: {discount}%");
        discount.Should().Be(20); // VIP(10%) + 高價商品(5%) + 購買歷史(5%) = 20%
    }

    [Fact]
    public void CalculateDiscount_一般客戶購買低價商品_應回傳0百分比折扣()
    {
        // Arrange
        var customer = new Customer { Type = CustomerType.Regular, PurchaseHistory = 1000 };
        var product = new Product { Price = 500, Category = "Accessories" };

        this._output.WriteLine($"Testing Regular customer: {customer.Type}, History: {customer.PurchaseHistory}");
        this._output.WriteLine($"Product: {product.Category}, Price: {product.Price}");

        var service = new ProductService();

        // Act
        var discount = service.CalculateDiscount(customer, product);

        // Assert
        this._output.WriteLine($"Calculated discount: {discount}%");
        discount.Should().Be(0);
    }
}