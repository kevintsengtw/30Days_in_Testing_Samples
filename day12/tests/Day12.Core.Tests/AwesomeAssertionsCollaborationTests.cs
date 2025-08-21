using Day12.Core.Tests.Attributes;
using Day12.Core.Tests.DataSources;

namespace Day12.Core.Tests;

/// <summary>
/// AutoData 與 Awesome Assertions 協作測試
/// </summary>
public class AwesomeAssertionsCollaborationTests
{
    // 代理方法讓 MemberAutoData 能找到正確的資料來源
    public static IEnumerable<object[]> ElectronicsFromCsv()
    {
        return ProductTestDataSource.ElectronicsFromCsv();
    }

    [Theory]
    [InlineAutoData("VIP", 100000)]
    [InlineAutoData("Premium", 50000)]
    [InlineAutoData("Regular", 20000)]
    public void AutoData與AwesomeAssertions協作_客戶等級與信用額度驗證(
        string customerLevel,
        decimal expectedCreditLimit,
        [Range(1000, 15000)] decimal orderAmount, // 確保所有等級客戶都能負擔
        Customer customer,
        Order order)
    {
        // Arrange
        customer.Type = customerLevel;
        customer.CreditLimit = expectedCreditLimit;
        order.Amount = orderAmount;

        // Act
        var canPlaceOrder = customer.CanPlaceOrder(order.Amount);
        var discountRate = CalculateDiscount(customer.Type, order.Amount);

        // Assert - 使用 Awesome Assertions 語法
        customer.Type.Should().Be(customerLevel);
        customer.CreditLimit.Should().Be(expectedCreditLimit);
        customer.CreditLimit.Should().BePositive();

        order.Amount.Should().BeInRange(1000m, 15000m);

        // 驗證下單能力（訂單金額在所有客戶等級的信用額度內）
        canPlaceOrder.Should().BeTrue();

        // 驗證折扣率範圍
        discountRate.Should().BeInRange(0m, 0.3m);
    }

    [Theory]
    [InlineAutoData("VIP", 0.15)]
    [InlineAutoData("Premium", 0.10)]
    [InlineAutoData("Regular", 0.05)]
    public void AutoData與AwesomeAssertions協作_VIP客戶折扣驗證(
        string customerLevel,
        decimal expectedBaseDiscount,
        [Range(1000, 25000)] decimal orderAmount,
        Customer customer,
        Order order)
    {
        // Arrange
        customer.Type = customerLevel;
        customer.CreditLimit = 100000m;
        order.Amount = orderAmount;

        // Act
        var discountRate = CalculateDiscount(customer.Type, order.Amount);

        // Assert
        discountRate.Should().BeGreaterThanOrEqualTo(expectedBaseDiscount);

        // VIP 客戶的特殊驗證
        discountRate.Should().Be(expectedBaseDiscount);
    }

    [Theory]
    [InlineAutoData("VIP", 35000, 0.20)]
    [InlineAutoData("Premium", 40000, 0.15)]
    [InlineAutoData("Regular", 35000, 0.10)]
    public void AutoData與AwesomeAssertions協作_大額訂單額外折扣驗證(
        string customerLevel,
        decimal largeOrderAmount,
        decimal expectedDiscountRate,
        Customer customer,
        Order order)
    {
        // Arrange
        customer.Type = customerLevel;
        customer.CreditLimit = 100000m;
        order.Amount = largeOrderAmount;

        // Act
        var discountRate = CalculateDiscount(customer.Type, order.Amount);

        // Assert
        order.Amount.Should().BeGreaterThan(30000m);
        discountRate.Should().Be(expectedDiscountRate);
    }

    [Theory]
    [MemberAutoData(nameof(ElectronicsFromCsv))]
    public void 複雜業務場景驗證_電子產品訂單處理(
        int productId,
        string productName,
        string category,
        decimal price,
        bool isAvailable,
        [CollectionSize(3)] List<Customer> customers,
        Order order)
    {
        // Arrange
        var product = new Product
        {
            Name = productName,
            Price = price,
            IsAvailable = isAvailable
        };

        var vipCustomer = customers.First();
        vipCustomer.Type = "VIP";
        vipCustomer.CreditLimit = 200000m;

        // Act
        var orderResult = ProcessElectronicsOrder(vipCustomer, product, order, quantity: 2);

        // Assert - 使用 Awesome Assertions 驗證複雜結果
        productId.Should().BePositive(); // 使用 productId 參數
        category.Should().Be("3C產品");    // 使用 category 參數
        orderResult.Should().NotBeNull();
        orderResult.IsSuccess.Should().BeTrue();

        // 驗證產品資訊
        orderResult.Product.Should().NotBeNull();
        orderResult.Product.Name.Should().Be(productName);
        orderResult.Product.Price.Should().Be(price);

        // 驗證客戶資訊
        orderResult.Customer.Should().NotBeNull();
        orderResult.Customer.Type.Should().Be("VIP");
        orderResult.Customer.CreditLimit.Should().Be(200000m);

        // 驗證訂單計算
        orderResult.TotalAmount.Should().Be(price * 2);       // 數量 x 單價
        orderResult.DiscountAmount.Should().BeGreaterThan(0); // VIP 客戶應有折扣
        orderResult.FinalAmount.Should().BeLessThan(orderResult.TotalAmount);

        // 驗證集合資料
        customers.Should().HaveCount(3);
        customers.Should().AllSatisfy(customer =>
        {
            customer.Person.Should().NotBeNull();
            customer.Person.Name.Should().NotBeNullOrEmpty();
            customer.CreditLimit.Should().BePositive();
        });
    }

    [Theory]
    [InlineAutoData("3C產品", 15000, true, 5)]
    [InlineAutoData("3C產品", 25000, true, 7)]
    public void 複雜業務場景驗證_高價3C產品需要審核(
        string category,
        decimal price,
        bool expectedRequiresApproval,
        int maxDeliveryDays,
        [CollectionSize(1)] List<Customer> customers,
        Order order)
    {
        // Arrange
        var product = new Product
        {
            Name = $"{category}產品",
            Price = price,
            IsAvailable = true
        };

        var vipCustomer = customers.First();
        vipCustomer.Type = "VIP";
        vipCustomer.CreditLimit = 200000m;

        // Act
        var orderResult = ProcessElectronicsOrder(vipCustomer, product, order, quantity: 1);

        // Assert
        category.Should().Be("3C產品");
        price.Should().BeGreaterThan(10000m);
        orderResult.RequiresApproval.Should().Be(expectedRequiresApproval);
        orderResult.EstimatedDeliveryDays.Should().BeInRange(3, maxDeliveryDays);
    }

    /// <summary>
    /// 計算折扣率的輔助方法，根據客戶類型和訂單金額計算折扣率
    /// </summary>
    /// <param name="customerType">customerType</param>
    /// <param name="orderAmount">orderAmount</param>
    /// <returns></returns>
    private static decimal CalculateDiscount(string customerType, decimal orderAmount)
    {
        var baseDiscount = customerType switch
        {
            "VIP" => 0.15m,
            "Premium" => 0.10m,
            "Regular" => 0.05m,
            _ => 0m
        };

        // 大額訂單額外折扣
        var largeOrderBonus = orderAmount > 30000m ? 0.05m : 0m;

        return Math.Min(baseDiscount + largeOrderBonus, 0.3m); // 最高 30% 折扣
    }

    /// <summary>
    /// 處理電子產品訂單的複雜業務邏輯
    /// </summary>
    /// <param name="customer">customer</param>
    /// <param name="product">product</param>
    /// <param name="order">order</param>
    /// <param name="quantity">quantity</param>
    /// <returns></returns>
    private static OrderResult ProcessElectronicsOrder(Customer customer, Product product, Order order, int quantity)
    {
        var totalAmount = product.Price * quantity;
        var discountRate = customer.Type == "VIP" ? 0.15m : 0.1m;
        var discountAmount = totalAmount * discountRate;
        var finalAmount = totalAmount - discountAmount;

        return new OrderResult
        {
            IsSuccess = product.IsAvailable && finalAmount <= customer.CreditLimit,
            Product = product,
            Customer = customer,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            RequiresApproval = product.Price > 10000m,
            EstimatedDeliveryDays = product.Price > 10000m ? Random.Shared.Next(3, 8) : Random.Shared.Next(1, 4)
        };
    }
}