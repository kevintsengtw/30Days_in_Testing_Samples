namespace Day27.Core.Tests.Services;

/// <summary>
/// 折扣計算器測試類別
/// </summary>
public class DiscountCalculatorTests
{
    [Fact(DisplayName = "計算折扣後的價格: 輸入有效的價格和折扣率，應回傳正確的折扣價格")]
    public void CalculateDiscountedPrice_輸入有效價格和折扣率_應回傳正確折扣價格()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var discountRate = 0.2m;
        var expected = 80m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入零折扣率，應回傳原始價格不變")]
    public void CalculateDiscountedPrice_輸入零折扣率_應回傳原始價格不變()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 50m;
        var discountRate = 0m;
        var expected = 50m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入全額折扣率，應回傳零價格")]
    public void CalculateDiscountedPrice_輸入全額折扣率_應回傳零價格()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var discountRate = 1m;
        var expected = 0m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入小數價格，應正確計算小數精度")]
    public void CalculateDiscountedPrice_輸入小數價格_應正確計算小數精度()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 99.99m;
        var discountRate = 0.15m;
        var expected = 84.9915m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入小數折扣率，應正確計算折扣精度")]
    public void CalculateDiscountedPrice_輸入小數折扣率_應正確計算折扣精度()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var discountRate = 0.333m;
        var expected = 66.7m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入零價格，應回傳零結果")]
    public void CalculateDiscountedPrice_輸入零價格_應回傳零結果()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 0m;
        var discountRate = 0.5m;
        var expected = 0m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入最小折扣率邊界值，應正確處理")]
    public void CalculateDiscountedPrice_輸入最小折扣率邊界值_應正確處理()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var discountRate = 0.0m;
        var expected = 100m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入最大折扣率邊界值，應正確處理")]
    public void CalculateDiscountedPrice_輸入最大折扣率邊界值_應正確處理()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var discountRate = 1.0m;
        var expected = 0m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入極小價格值，應保持計算精度")]
    public void CalculateDiscountedPrice_輸入極小價格值_應保持計算精度()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 0.01m;
        var discountRate = 0.1m;
        var expected = 0.009m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入極大價格值，應保持計算穩定性")]
    public void CalculateDiscountedPrice_輸入極大價格值_應保持計算穩定性()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 999999.99m;
        var discountRate = 0.1m;
        var expected = 899999.991m;

        // Act
        var actual = calculator.CalculateDiscountedPrice(originalPrice, discountRate);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入負數原始價格，應拋出ArgumentException")]
    public void CalculateDiscountedPrice_輸入負數原始價格_應拋出ArgumentException()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = -100m;
        var discountRate = 0.2m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            calculator.CalculateDiscountedPrice(originalPrice, discountRate));
        
        exception.ParamName.Should().Be("originalPrice");
        exception.Message.Should().Contain("原始價格不能為負數");
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入負數折扣率，應拋出ArgumentException")]
    public void CalculateDiscountedPrice_輸入負數折扣率_應拋出ArgumentException()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var discountRate = -0.1m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            calculator.CalculateDiscountedPrice(originalPrice, discountRate));
        
        exception.ParamName.Should().Be("discountRate");
        exception.Message.Should().Contain("折扣率必須在 0 到 1 之間");
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入超過1的折扣率，應拋出ArgumentException")]
    public void CalculateDiscountedPrice_輸入超過1的折扣率_應拋出ArgumentException()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var discountRate = 1.5m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            calculator.CalculateDiscountedPrice(originalPrice, discountRate));
        
        exception.ParamName.Should().Be("discountRate");
        exception.Message.Should().Contain("折扣率必須在 0 到 1 之間");
    }

    [Fact(DisplayName = "計算折扣後的價格: 輸入極端負數價格，應拋出ArgumentException")]
    public void CalculateDiscountedPrice_輸入極端負數價格_應拋出ArgumentException()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = -999999.99m;
        var discountRate = 0.1m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            calculator.CalculateDiscountedPrice(originalPrice, discountRate));
        
        exception.ParamName.Should().Be("originalPrice");
        exception.Message.Should().Contain("原始價格不能為負數");
    }

    // CalculateBulkDiscount 方法測試

    [Fact(DisplayName = "計算批量折扣: 小量購買（無折扣），應回傳原始總價")]
    public void CalculateBulkDiscount_小量購買無折扣_應回傳原始總價()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 5;
        var expected = 50m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 中等量購買（5% 折扣），應回傳正確折扣總價")]
    public void CalculateBulkDiscount_中等量購買_百分之5折扣_應回傳正確折扣總價()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 15;
        var expected = 142.5m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 大量購買（10% 折扣），應回傳正確折扣總價")]
    public void CalculateBulkDiscount_大量購買_百分之10折扣_應回傳正確折扣總價()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 60;
        var expected = 540m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 批量購買（15% 折扣），應回傳正確折扣總價")]
    public void CalculateBulkDiscount_批量購買_百分之15折扣_應回傳正確折扣總價()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 120;
        var expected = 1020m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 單一商品購買，應回傳原始價格")]
    public void CalculateBulkDiscount_單一商品購買_應回傳原始價格()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 100m;
        var quantity = 1;
        var expected = 100m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 折扣臨界值（10個），應正確處理5%折扣門檻")]
    public void CalculateBulkDiscount_折扣臨界值_10個_應正確處理百分之5折扣門檻()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 10;
        var expected = 95m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 折扣臨界值（50個），應正確處理10%折扣門檻")]
    public void CalculateBulkDiscount_折扣臨界值_50個_應正確處理百分之10折扣門檻()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 50;
        var expected = 450m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 折扣臨界值（100個），應正確處理15%折扣門檻")]
    public void CalculateBulkDiscount_折扣臨界值_100個_應正確處理百分之15折扣門檻()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 100;
        var expected = 850m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 小數價格批量購買，應保持計算精度")]
    public void CalculateBulkDiscount_小數價格批量購買_應保持計算精度()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 99.99m;
        var quantity = 25;
        var expected = 2374.7625m; // 99.99 * 25 * (1 - 0.05) = 2499.75 * 0.95 = 2374.7625

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 極大數量購買，應保持計算穩定性")]
    public void CalculateBulkDiscount_極大數量購買_應保持計算穩定性()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 1m;
        var quantity = 10000;
        var expected = 8500m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact(DisplayName = "計算批量折扣: 輸入負數原始價格，應拋出ArgumentException")]
    public void CalculateBulkDiscount_輸入負數原始價格_應拋出ArgumentException()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = -10m;
        var quantity = 5;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            calculator.CalculateBulkDiscount(originalPrice, quantity));
        
        exception.ParamName.Should().Be("originalPrice");
        exception.Message.Should().Contain("原始價格不能為負數");
    }

    [Fact(DisplayName = "計算批量折扣: 輸入零數量，應拋出ArgumentException")]
    public void CalculateBulkDiscount_輸入零數量_應拋出ArgumentException()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = 0;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            calculator.CalculateBulkDiscount(originalPrice, quantity));
        
        exception.ParamName.Should().Be("quantity");
        exception.Message.Should().Contain("數量必須大於 0");
    }

    [Fact(DisplayName = "計算批量折扣: 輸入負數量，應拋出ArgumentException")]
    public void CalculateBulkDiscount_輸入負數量_應拋出ArgumentException()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 10m;
        var quantity = -5;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            calculator.CalculateBulkDiscount(originalPrice, quantity));
        
        exception.ParamName.Should().Be("quantity");
        exception.Message.Should().Contain("數量必須大於 0");
    }

    [Fact(DisplayName = "計算批量折扣: 輸入零價格正常數量，應回傳零結果")]
    public void CalculateBulkDiscount_輸入零價格正常數量_應回傳零結果()
    {
        // Arrange
        var calculator = new DiscountCalculator();
        var originalPrice = 0m;
        var quantity = 10;
        var expected = 0m;

        // Act
        var actual = calculator.CalculateBulkDiscount(originalPrice, quantity);

        // Assert
        actual.Should().Be(expected);
    }
}