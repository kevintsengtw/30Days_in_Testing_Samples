using System;

namespace Day09.Core.Tests;

/// <summary>
/// PriceCalculator 測試類別（測試 Internal 成員）
/// </summary>
public class PriceCalculatorTests
{
    [Theory]
    [InlineData(15000, "豪華級")]
    [InlineData(8000, "高級")]
    [InlineData(3000, "中級")]
    [InlineData(500, "經濟級")]
    [InlineData(0, "無效價格")]
    public void CalculatePriceLevel_不同價格_應回傳正確等級(decimal price, string expected)
    {
        // Arrange
        var calculator = new PriceCalculator();

        // Act
        var actual = calculator.CalculatePriceLevel(price);

        // Assert
        AssertionExtensions.Should(actual).Be(expected);
    }

    [Theory]
    [InlineData(1000, 0.1, 900)]
    [InlineData(2000, 0.2, 1600)]
    [InlineData(500, 0.05, 475)]
    public void CalculateDiscountedPrice_正常折扣_應計算正確價格(
        decimal originalPrice, decimal discountRate, decimal expected)
    {
        // Arrange
        var calculator = new PriceCalculator();

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        AssertionExtensions.Should(actual).Be(expected);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void CalculateDiscountedPrice_無效折扣率_應拋出例外(decimal invalidDiscountRate)
    {
        // Arrange
        var calculator = new PriceCalculator();

        // Act & Assert
        var action = () => calculator.CalculateDiscountedPrice(1000, invalidDiscountRate);
        action.Should().Throw<ArgumentException>()
              .WithMessage("折扣率必須在0到1之間");
    }
}