namespace TUnit.Demo.Tests;

/// <summary>
/// Calculator 類別的測試案例
/// </summary>
public class CalculatorTests
{
    private readonly Calculator _calculator;

    public CalculatorTests()
    {
        _calculator = new Calculator();
    }

    #region 基本測試

    [Test]
    public async Task Add_輸入1和2_應回傳3()
    {
        // Arrange
        var a = 1;
        var b = 2;
        var expected = 3;

        // Act
        var result = _calculator.Add(a, b);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task Divide_輸入0作為除數_應拋出DivideByZeroException()
    {
        // Arrange
        var dividend = 10;
        var divisor = 0;

        // Act & Assert
        await Assert.That(() => _calculator.Divide(dividend, divisor))
                    .Throws<DivideByZeroException>();
    }

    #endregion

    #region 參數化測試

    [Test]
    [Arguments(1, 2, 3)]
    [Arguments(-1, 1, 0)]
    [Arguments(0, 0, 0)]
    [Arguments(100, -50, 50)]
    public async Task Add_多組輸入_應回傳正確結果(int a, int b, int expected)
    {
        // Act
        var result = _calculator.Add(a, b);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    [Arguments(1, true)]
    [Arguments(-1, false)]
    [Arguments(0, false)]
    [Arguments(100, true)]
    public async Task IsPositive_各種數值_應回傳正確結果(int number, bool expected)
    {
        // Act
        var result = _calculator.IsPositive(number);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    #endregion
}