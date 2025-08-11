namespace Day02.Core.Tests;

public class CalculatorTests
{
    private readonly Calculator _calculator;

    /// <summary>
    /// 建構式：每個測試都會建立新的 Calculator 實例
    /// </summary>
    public CalculatorTests()
    {
        this._calculator = new Calculator();
    }

    //---------------------------------------------------------------------------------------------
    // Add 方法測試

    [Fact]
    public void Add_輸入兩個正數_應回傳正確的和()
    {
        // Arrange
        const int a = 5;
        const int b = 3;
        const int expected = 8;

        // Act
        var actual = this._calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(0, 0, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-5, -3, -8)]
    [InlineData(100, -50, 50)]
    public void Add_各種數字組合_應回傳正確結果(int a, int b, int expected)
    {
        // Act
        var result = this._calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    //---------------------------------------------------------------------------------------------
    // Subtract 方法測試

    [Fact]
    public void Subtract_輸入被減數大於減數_應回傳正數()
    {
        // Arrange
        const int minuend = 10;
        const int subtrahend = 3;
        const int expected = 7;

        // Act
        var actual = this._calculator.Subtract(minuend, subtrahend);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(5, 5, 0)]
    [InlineData(3, 10, -7)]
    [InlineData(0, 5, -5)]
    [InlineData(-5, -3, -2)]
    public void Subtract_各種數字組合_應回傳正確結果(int a, int b, int expected)
    {
        // Act
        var actual = this._calculator.Subtract(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    //---------------------------------------------------------------------------------------------
    // Multiply 方法測試

    [Fact]
    public void Multiply_輸入兩個正數_應回傳正確的積()
    {
        // Arrange
        const int a = 4;
        const int b = 5;
        const int expected = 20;

        // Act
        var actual = this._calculator.Multiply(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 5, 0)]    // 零乘以任何數都是零
    [InlineData(1, 7, 7)]    // 一乘以任何數都是該數
    [InlineData(-3, 4, -12)] // 負數乘以正數
    [InlineData(-2, -5, 10)] // 負數乘以負數
    public void Multiply_特殊情況_應回傳正確結果(int a, int b, int expected)
    {
        // Act
        var actual = this._calculator.Multiply(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    //---------------------------------------------------------------------------------------------
    // Divide 方法測試

    [Fact]
    public void Divide_輸入有效的被除數和除數_應回傳正確商()
    {
        // Arrange
        const int dividend = 10;
        const int divisor = 2;
        const double expected = 5.0;

        // Act
        var actual = this._calculator.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(10, 2, 5.0)]
    [InlineData(7, 2, 3.5)]
    [InlineData(-10, 2, -5.0)]
    [InlineData(10, -2, -5.0)]
    [InlineData(0, 5, 0.0)]
    public void Divide_各種有效輸入_應回傳正確結果(int dividend, int divisor, double expected)
    {
        // Act
        var actual = this._calculator.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expected, actual, precision: 2); // 指定精確度
    }

    [Fact]
    public void Divide_除數為零_應拋出DivideByZeroException()
    {
        // Arrange
        const int dividend = 10;
        const int divisor = 0;

        // Act & Assert
        var exception = Assert.Throws<DivideByZeroException>(() => this._calculator.Divide(dividend, divisor));

        // 驗證例外訊息
        Assert.Equal("除數不能為零", exception.Message);
    }

    //---------------------------------------------------------------------------------------------
    // IsEven 方法測試

    [Theory]
    [InlineData(2, true)]
    [InlineData(4, true)]
    [InlineData(0, true)]  // 零是偶數
    [InlineData(-2, true)] // 負偶數
    [InlineData(1, false)]
    [InlineData(3, false)]
    [InlineData(-1, false)] // 負奇數
    [InlineData(-3, false)]
    public void IsEven_各種整數輸入_應正確判斷奇偶(int number, bool expected)
    {
        // Act
        var actual = this._calculator.IsEven(number);

        // Assert
        Assert.Equal(expected, actual);
    }
}