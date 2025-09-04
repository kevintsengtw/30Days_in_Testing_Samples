namespace Calculator.Tests.V3;

/// <summary>
/// 展示 xUnit v3 測試屬性和 API
/// 注意：某些特性可能在實際版本中有所不同
/// </summary>
public class TestAttributeAndApiTests
{
    private readonly Core.Calculator _calculator;

    public TestAttributeAndApiTests()
    {
        _calculator = new Core.Calculator();
    }

    /// <summary>
    /// 展示 [Fact] 測試基本用法
    /// </summary>
    [Fact]
    public void Add_兩個正數_應回傳正確結果()
    {
        // Arrange
        var a = 5;
        var b = 3;
        var expected = 8;

        // Act
        var actual = _calculator.Add(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 展示基本計算功能測試
    /// </summary>
    [Theory]
    [InlineData(10, 5, 15)]
    [InlineData(-3, 7, 4)]
    [InlineData(0, 0, 0)]
    [InlineData(-5, -3, -8)]
    public void Add_各種數值組合_應回傳正確結果(int a, int b, int expected)
    {
        // Arrange & Act
        var actual = _calculator.Add(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 展示除法測試
    /// </summary>
    [Fact]
    public void Divide_正常除法_應回傳正確結果()
    {
        // Arrange
        var dividend = 10;
        var divisor = 2;
        var expected = 5.0;

        // Act
        var actual = _calculator.Divide(dividend, divisor);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 展示除零例外測試
    /// </summary>
    [Fact]
    public void Divide_除數為零_應拋出例外()
    {
        // Arrange
        var dividend = 10;
        var divisor = 0;

        // Act & Assert
        var exception = Assert.Throws<DivideByZeroException>(() => _calculator.Divide(dividend, divisor));
        exception.Message.Should().Be("除數不能為零");
    }

    /// <summary>
    /// 展示乘法測試
    /// </summary>
    [Theory]
    [InlineData(3, 4, 12)]
    [InlineData(-2, 5, -10)]
    [InlineData(0, 100, 0)]
    [InlineData(-3, -4, 12)]
    public void Multiply_各種數值組合_應回傳正確結果(int a, int b, int expected)
    {
        // Arrange & Act
        var actual = _calculator.Multiply(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 展示減法測試
    /// </summary>
    [Fact]
    public void Subtract_正常減法_應回傳正確結果()
    {
        // Arrange
        var minuend = 10;
        var subtrahend = 3;
        var expected = 7;

        // Act
        var actual = _calculator.Subtract(minuend, subtrahend);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 展示複雜計算測試
    /// </summary>
    [Fact]
    public void ComplexCalculation_多步驟計算_應回傳正確結果()
    {
        // Arrange
        var step1 = _calculator.Add(10, 5);         // 15
        var step2 = _calculator.Multiply(step1, 2); // 30
        var step3 = _calculator.Subtract(step2, 8); // 22
        var step4 = _calculator.Divide(step3, 2);   // 11

        // Act & Assert
        step1.Should().Be(15);
        step2.Should().Be(30);
        step3.Should().Be(22);
        step4.Should().Be(11.0);
    }

    /// <summary>
    /// 展示邊界值測試
    /// </summary>
    [Theory]
    [InlineData(int.MaxValue, 0, int.MaxValue)]
    [InlineData(int.MinValue, 0, int.MinValue)]
    [InlineData(0, int.MaxValue, int.MaxValue)]
    [InlineData(0, int.MinValue, int.MinValue)]
    public void Add_邊界值測試_應正確處理(int a, int b, int expected)
    {
        // Arrange & Act
        var actual = _calculator.Add(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 展示浮點數精度測試
    /// </summary>
    [Fact]
    public void Divide_浮點數精度_應在可接受範圍內()
    {
        // Arrange
        var dividend = 1;
        var divisor = 3;
        var expected = 0.333333333333333; // 1/3 的近似值

        // Act
        var actual = _calculator.Divide(dividend, divisor);

        // Assert
        Math.Abs(actual - expected).Should().BeLessThan(1e-10);
    }
}