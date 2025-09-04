namespace Calculator.Tests.V3;

/// <summary>
/// 計算機測試類別 - xUnit 3.x 版本
/// 展示了 xUnit 3.x 的新功能和最佳實作
/// </summary>
public class CalculatorTests
{
    private readonly Core.Calculator _calculator;

    public CalculatorTests()
    {
        _calculator = new Core.Calculator();
    }

    /// <summary>
    /// 測試兩個正整數相加
    /// </summary>
    [Fact]
    public void Add_兩個正整數_應回傳正確總和()
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
    /// 測試零相加
    /// </summary>
    [Fact]
    public void Add_其中一個數為零_應回傳另一個數()
    {
        // Arrange
        var a = 10;
        var b = 0;
        var expected = 10;

        // Act
        var actual = _calculator.Add(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 測試減法運算
    /// </summary>
    [Fact]
    public void Subtract_正常減法_應回傳正確結果()
    {
        // Arrange
        var a = 10;
        var b = 4;
        var expected = 6;

        // Act
        var actual = _calculator.Subtract(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 測試乘法運算，使用基本 Theory 展示多組資料測試
    /// </summary>
    [Theory]
    [InlineData(2, 3, 6)]
    [InlineData(-2, 3, -6)]
    [InlineData(0, 5, 0)]
    public void Multiply_各種整數相乘_應回傳正確結果(int a, int b, int expected)
    {
        // Arrange

        // Act
        var actual = _calculator.Multiply(a, b);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 測試除法運算
    /// </summary>
    [Fact]
    public void Divide_正常除法_應回傳正確結果()
    {
        // Arrange
        var dividend = 15;
        var divisor = 3;
        var expected = 5;

        // Act
        var actual = _calculator.Divide(dividend, divisor);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 測試除零例外
    /// </summary>
    [Fact]
    public void Divide_除以零_應拋出例外()
    {
        // Arrange
        var dividend = 10;
        var divisor = 0;

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => _calculator.Divide(dividend, divisor));
    }

    /// <summary>
    /// 測試判斷奇偶數
    /// </summary>
    [Theory]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(-3, true)]
    [InlineData(0, false)]
    public void IsOdd_各種整數_應正確判斷奇偶(int number, bool expected)
    {
        // Arrange

        // Act
        var actual = _calculator.IsOdd(number);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 測試平方運算
    /// </summary>
    [Theory]
    [InlineData(3, 9)]
    [InlineData(-4, 16)]
    [InlineData(0, 0)]
    public void Square_各種整數的平方_應回傳正確結果(int number, int expected)
    {
        // Arrange

        // Act
        var actual = _calculator.Square(number);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 測試非同步費氏數列計算
    /// 注意：xUnit 3.x 要求 async Task 而不是 async void
    /// </summary>
    [Fact]
    public async Task CalculateFibonacciAsync_計算費氏數列_應回傳正確結果()
    {
        // Arrange
        var n = 6;
        var expected = 8; // F(6) = 8

        // Act
        var actual = await _calculator.CalculateFibonacciAsync(n);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// 測試負數的費氏數列
    /// </summary>
    [Fact]
    public async Task CalculateFibonacciAsync_負數輸入_應拋出例外()
    {
        // Arrange
        var n = -1;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            async () => await _calculator.CalculateFibonacciAsync(n));
    }

    /// <summary>
    /// 展示新的測試資料提供方式
    /// </summary>
    public static TheoryData<int, int, int> AdditionTestData =>
        new()
        {
            { 1, 2, 3 },
            { -1, 1, 0 },
            { 0, 0, 0 },
            { 100, -50, 50 }
        };

    /// <summary>
    /// 使用自訂測試資料的 Theory 測試
    /// </summary>
    [Theory]
    [MemberData(nameof(AdditionTestData))]
    public void Add_使用自訂測試資料_應回傳正確結果(int a, int b, int expected)
    {
        // Arrange

        // Act
        var actual = _calculator.Add(a, b);

        // Assert
        actual.Should().Be(expected);
    }
}