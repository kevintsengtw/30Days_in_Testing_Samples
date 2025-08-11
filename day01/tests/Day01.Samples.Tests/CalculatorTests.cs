using Day01.Core;

namespace Day01.Samples.Tests;

/// <summary>
/// class CalculatorTests - Calculator 測試類別
/// </summary>
public class CalculatorTests
{
    private readonly Calculator _calculator;

    public CalculatorTests()
    {
        this._calculator = new Calculator();
    }

    //---------------------------------------------------------------------------------------------
    // Add 方法測試 - 展示基本的 FIRST 原則

    [Fact] // Fast: 不依賴外部資源，執行快速
    public void Add_輸入1和2_應回傳3()
    {
        // Arrange
        const int a = 1;
        const int b = 2;
        const int expected = 3;

        // Act
        var result = this._calculator.Add(a, b);

        // Assert - Self-Validating: 明確的驗證和錯誤訊息
        Assert.Equal(expected, result);
    }

    [Fact] // Independent: 不依賴其他測試的結果
    public void Add_輸入負數和正數_應回傳正確結果()
    {
        // Arrange
        const int a = -5;
        const int b = 3;
        const int expected = -2;

        // Act
        var result = this._calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact] // Repeatable: 每次執行都會得到相同結果
    public void Add_輸入0和0_應回傳0()
    {
        // Arrange
        const int a = 0;
        const int b = 0;
        const int expected = 0;

        // Act
        var result = this._calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    //---------------------------------------------------------------------------------------------
    // Theory 測試 - 一次測試多個案例

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(-1, 1, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(100, -50, 50)]
    public void Add_輸入各種數值組合_應回傳正確結果(int a, int b, int expected)
    {
        // Act
        var result = this._calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    //---------------------------------------------------------------------------------------------
    // Divide 方法測試 - 展示例外處理

    [Fact]
    public void Divide_輸入10和2_應回傳5()
    {
        // Arrange
        const decimal dividend = 10m;
        const decimal divisor = 2m;
        const decimal expected = 5m;

        // Act
        var result = this._calculator.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact] // 測試例外情況
    public void Divide_輸入10和0_應拋出DivideByZeroException()
    {
        // Arrange
        const decimal dividend = 10m;
        const decimal divisor = 0m;

        // Act & Assert
        var exception = Assert.Throws<DivideByZeroException>(() => this._calculator.Divide(dividend, divisor));

        Assert.Equal("除數不能為零", exception.Message);
    }

    [Theory]
    [InlineData(10, 2, 5)]
    [InlineData(15, 3, 5)]
    [InlineData(7, 2, 3.5)]
    [InlineData(-10, 2, -5)]
    public void Divide_輸入各種有效數值_應回傳正確結果(decimal dividend, decimal divisor, decimal expected)
    {
        // Act
        var result = this._calculator.Divide(dividend, divisor);

        // Assert
        Assert.Equal(expected, result);
    }

    //---------------------------------------------------------------------------------------------
    // Multiply 方法測試

    [Fact]
    public void Multiply_輸入3和4_應回傳12()
    {
        // Arrange
        const int a = 3;
        const int b = 4;
        const int expected = 12;

        // Act
        var result = this._calculator.Multiply(a, b);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 5, 0)]
    [InlineData(1, 1, 1)]
    [InlineData(-2, 3, -6)]
    [InlineData(-2, -3, 6)]
    public void Multiply_輸入各種數值組合_應回傳正確結果(int a, int b, int expected)
    {
        // Act
        var result = this._calculator.Multiply(a, b);

        // Assert
        Assert.Equal(expected, result);
    }
}