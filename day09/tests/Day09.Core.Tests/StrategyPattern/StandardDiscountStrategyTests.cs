using System;
using Day09.Core.StrategyPattern;

namespace Day09.Core.Tests.StrategyPattern;

/// <summary>
/// StandardDiscountStrategy 測試類別
/// </summary>
public class StandardDiscountStrategyTests
{
    [Fact]
    public void Calculate_VIP客戶_應給予折扣()
    {
        // Arrange
        var strategy = new StandardDiscountStrategy();
        var customer = new Customer { IsVip = true };
        var product = new Product { BasePrice = 1000m };

        // Act
        var discount = strategy.Calculate(customer, product);

        // Assert
        discount.Should().Be(100m);
    }

    [Fact]
    public void Calculate_一般客戶_應無折扣()
    {
        // Arrange
        var strategy = new StandardDiscountStrategy();
        var customer = new Customer { IsVip = false };
        var product = new Product { BasePrice = 1000m };

        // Act
        var discount = strategy.Calculate(customer, product);

        // Assert
        discount.Should().Be(0m);
    }
}