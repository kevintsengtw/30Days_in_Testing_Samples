using System.Globalization;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace Day12.Core.Tests;

/// <summary>
/// 外部測試資料整合測試
/// </summary>
public class ExternalDataIntegrationTests
{
    public static IEnumerable<object[]> GetProductsFromCsv()
    {
        var testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "products.csv");

        var filePath = testDataPath;

        using var reader = new StringReader(File.ReadAllText(filePath));
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        var records = csv.GetRecords<ProductCsvRecord>().ToList();

        foreach (var record in records)
        {
            yield return
            [
                record.ProductId,
                record.Name,
                record.Category,
                record.Price,
                record.IsAvailable
            ];
        }
    }

    public static IEnumerable<object[]> GetCustomersFromJson()
    {
        var testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "customers.json");

        var filePath = testDataPath;
        var jsonContent = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var customers = JsonSerializer.Deserialize<List<CustomerJsonRecord>>(jsonContent, options);

        foreach (var customer in customers ?? new List<CustomerJsonRecord>())
        {
            yield return
            [
                customer.CustomerId,
                customer.Name,
                customer.Email,
                customer.Level,
                customer.CreditLimit
            ];
        }
    }

    [Theory]
    [MemberAutoData(nameof(GetProductsFromCsv))]
    public void CSV整合測試_產品資料驗證(
        int productId,
        string name,
        string category,
        decimal price,
        bool isAvailable,
        Order order) // 由 AutoFixture 自動產生
    {
        // Arrange
        var product = new Product
        {
            Name = name,
            Price = price,
            IsAvailable = isAvailable
        };

        order.Amount = price;

        // Act
        var orderItem = new OrderItem
        {
            ProductId = productId,
            Product = product,
            Quantity = 1
        };

        // Assert
        orderItem.ProductId.Should().Be(productId);
        orderItem.Product.Name.Should().Be(name);
        orderItem.Product.Price.Should().Be(price);

        // 驗證產品類別
        category.Should().NotBeNullOrEmpty("產品應該有類別");

        // 從 CSV 讀取的產品價格都應該是正數
        orderItem.Product.Price.Should().BePositive();
    }

    [Theory]
    [MemberAutoData(nameof(GetCustomersFromJson))]
    public void JSON整合測試_客戶資料驗證(
        int customerId,
        string name,
        string email,
        string level,
        decimal creditLimit,
        Order order) // 由 AutoFixture 自動產生
    {
        // Arrange
        var customer = new Customer
        {
            Person = new Person { Name = name, Email = email },
            Type = level,
            CreditLimit = creditLimit
        };

        order.Amount = 15000;
        var canPlaceOrder = customer.CanPlaceOrder(order.Amount);

        // Assert
        customer.Person.Name.Should().Be(name);
        customer.Person.Email.Should().Be(email);
        customer.Type.Should().Be(level);
        customer.CreditLimit.Should().Be(creditLimit);

        // 驗證客戶 ID
        customerId.Should().BePositive("客戶 ID 應該是正數");

        // 驗證客戶等級
        customer.Type.Should().BeOneOf("VIP", "Premium", "Regular");

        // 根據信用額度驗證下單能力
        order.Amount.Should().Be(15000);
        canPlaceOrder.Should().BeTrue();

        if (order.Amount <= customer.CreditLimit)
        {
            canPlaceOrder.Should().BeTrue();
        }
        else
        {
            canPlaceOrder.Should().BeFalse();
        }
    }
}