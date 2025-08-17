using System;
using Day09.Core.StrategyPattern;

namespace Day09.Core.Tests.StrategyPattern;

/// <summary>
/// PricingService 測試類別
/// </summary>
public class PricingServiceTests
{
    [Fact]
    public void CalculatePrice_VIP客戶_應正確計算最終價格()
    {
        // Arrange
        var discountStrategy = new StandardDiscountStrategy();
        var taxStrategy = new TaiwanTaxStrategy();
        var service = new PricingService(discountStrategy, taxStrategy);

        var customer = new Customer { IsVip = true };
        var product = new Product { BasePrice = 1000m };

        // Act
        var finalPrice = service.CalculatePrice(product, customer);

        // Assert
        // 1000 - 100 (10% VIP discount) + 50 (5% tax) = 950
        finalPrice.Should().Be(950m);
    }

    [Fact]
    public void CalculatePrice_一般客戶_應正確計算最終價格()
    {
        // Arrange
        var discountStrategy = new StandardDiscountStrategy();
        var taxStrategy = new TaiwanTaxStrategy();
        var service = new PricingService(discountStrategy, taxStrategy);

        var customer = new Customer { IsVip = false };
        var product = new Product { BasePrice = 1000m };

        // Act
        var finalPrice = service.CalculatePrice(product, customer);

        // Assert
        // 1000 - 0 (no discount) + 50 (5% tax) = 1050
        finalPrice.Should().Be(1050m);
    }

    [Fact]
    public void CalculatePrice_使用Mock策略_應正確整合()
    {
        // Arrange
        var mockDiscountStrategy = Substitute.For<IDiscountStrategy>();
        var mockTaxStrategy = Substitute.For<ITaxStrategy>();

        mockDiscountStrategy.Calculate(Arg.Any<Customer>(), Arg.Any<Product>())
                            .Returns(200m);

        mockTaxStrategy.Calculate(Arg.Any<Product>(), Arg.Any<Location>())
                       .Returns(75m);

        var service = new PricingService(mockDiscountStrategy, mockTaxStrategy);
        var customer = new Customer();
        var product = new Product { BasePrice = 1000m };

        // Act
        var finalPrice = service.CalculatePrice(product, customer);

        // Assert
        // 1000 - 200 (mocked discount) + 75 (mocked tax) = 875
        finalPrice.Should().Be(875m);

        // 驗證策略被正確呼叫
        mockDiscountStrategy.Received(1).Calculate(customer, product);
        mockTaxStrategy.Received(1).Calculate(product, customer.Location);
    }
}