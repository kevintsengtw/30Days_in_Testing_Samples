using System;
using Day09.Core.StrategyPattern;

namespace Day09.Core.Tests.StrategyPattern;

/// <summary>
/// TaiwanTaxStrategy 測試類別
/// </summary>
public class TaiwanTaxStrategyTests
{
    [Theory]
    [InlineData(1000, 50)]
    [InlineData(2000, 100)]
    [InlineData(500, 25)]
    public void Calculate_不同金額_應計算稅收(decimal basePrice, decimal expectedTax)
    {
        // Arrange
        var strategy = new TaiwanTaxStrategy();
        var product = new Product { BasePrice = basePrice };
        var location = new Location { Country = "Taiwan" };

        // Act
        var tax = strategy.Calculate(product, location);

        // Assert
        tax.Should().Be(expectedTax);
    }
}