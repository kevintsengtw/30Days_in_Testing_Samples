using Day12.Core.Tests.DataSources;

namespace Day12.Core.Tests;

/// <summary>
/// 資料來源設計模式測試
/// </summary>
public class DataSourceDesignPatternTests
{
    // 代理方法讓 MemberAutoData 能找到正確的資料來源
    public static IEnumerable<object[]> All()
    {
        return ReusableTestDataSets.ProductCategories.All();
    }

    public static IEnumerable<object[]> BasicProducts()
    {
        return ProductTestDataSource.BasicProducts();
    }

    public static IEnumerable<object[]> VipCustomers()
    {
        return CustomerTestDataSource.VipCustomers();
    }

    public static IEnumerable<object[]> Premium()
    {
        return ReusableTestDataSets.PriceRanges.Premium();
    }

    public static IEnumerable<object[]> Budget()
    {
        return ReusableTestDataSets.PriceRanges.Budget();
    }

    public static IEnumerable<object[]> ElectronicsFromCsv()
    {
        return ProductTestDataSource.ElectronicsFromCsv();
    }

    [Theory]
    [MemberAutoData(nameof(All))]
    public void 可重用資料集測試_產品分類驗證(
        string categoryName,
        string categoryCode,
        Product product)
    {
        // Arrange
        var categorizedProduct = new CategorizedProduct
        {
            Product = product,
            CategoryName = categoryName,
            CategoryCode = categoryCode
        };

        // Act
        var isValidCategory = ValidateCategory(categorizedProduct);

        // Assert
        isValidCategory.Should().BeTrue();
        ReusableTestDataSets.ProductCategories.All()
                            .SelectMany(data => data)
                            .Should().Contain(categoryName);
    }

    [Theory]
    [MemberAutoData(nameof(BasicProducts))]
    public void 階層式資料組織_基本產品測試(
        string productName,
        decimal price,
        bool isAvailable,
        Customer customer)
    {
        // Arrange
        var product = new Product
        {
            Name = productName,
            Price = price,
            IsAvailable = isAvailable
        };

        // Act
        var canPurchase = customer.CanPlaceOrder(price);

        // Assert
        product.Name.Should().Be(productName);
        product.Price.Should().Be(price);
        product.IsAvailable.Should().Be(isAvailable);

        // 基本產品的價格都應該是正數
        product.Price.Should().BePositive();
    }

    [Theory]
    [MemberAutoData(nameof(ElectronicsFromCsv))]
    public void 階層式資料組織_電子產品CSV測試(
        int productId,
        string name,
        string category,
        decimal price,
        bool isAvailable,
        Order order)
    {
        // Arrange
        var product = new Product
        {
            Name = name,
            Price = price,
            IsAvailable = isAvailable
        };

        // Act
        order.Amount = price;

        // Assert
        productId.Should().BePositive();
        category.Should().Be("3C產品");
        product.Price.Should().BeGreaterThan(1000m); // 3C產品通常價格較高
        order.Amount.Should().Be(price);
    }

    [Theory]
    [MemberAutoData(nameof(VipCustomers))]
    public void 階層式資料組織_VIP客戶測試(
        int customerId,
        string name,
        string email,
        string level,
        decimal creditLimit,
        Product product)
    {
        // Arrange
        var customer = new Customer
        {
            Person = new Person { Name = name, Email = email },
            Type = level,
            CreditLimit = creditLimit
        };

        // Act
        var canPurchaseExpensiveItem = customer.CanPlaceOrder(product.Price);

        // Assert
        customer.Type.Should().Be("VIP");
        customer.CreditLimit.Should().BeGreaterThan(30000m); // VIP客戶信用額度較高
        customer.Person.Email.Should().Contain("@");
        customerId.Should().BeGreaterThan(1000);
    }

    [Theory]
    [MemberAutoData(nameof(Budget))]
    public void 可重用資料集測試_預算價格區間(
        decimal minPrice,
        decimal maxPrice,
        Product product,
        Customer customer)
    {
        // Arrange
        product.Price = (minPrice + maxPrice) / 2; // 設定為區間中間值
        customer.CreditLimit = maxPrice * 2;       // 設定足夠的信用額度

        // Act
        var canAfford = customer.CanPlaceOrder(product.Price);

        // Assert
        minPrice.Should().BeLessThan(maxPrice);
        product.Price.Should().BeInRange(minPrice, maxPrice);
        canAfford.Should().BeTrue();
    }

    [Theory]
    [MemberAutoData(nameof(Premium))]
    public void 可重用資料集測試_高級價格區間(
        decimal minPrice,
        decimal maxPrice,
        Product product,
        Customer customer)
    {
        // Arrange
        product.Price = minPrice;        // 設定為最低高級價格
        customer.CreditLimit = maxPrice; // 設定為最高信用額度

        // Act
        var canAfford = customer.CanPlaceOrder(product.Price);

        // Assert
        minPrice.Should().BeGreaterThanOrEqualTo(5000m); // 高級價格區間的最低價
        maxPrice.Should().BeLessThanOrEqualTo(50000m);   // 高級價格區間的最高價
        canAfford.Should().BeTrue();
    }

    private static bool ValidateCategory(CategorizedProduct product)
    {
        var validCategories = ReusableTestDataSets.ProductCategories.All()
                                                  .Select(data => data[0].ToString()).ToList();
        return validCategories.Contains(product.CategoryName);
    }
}