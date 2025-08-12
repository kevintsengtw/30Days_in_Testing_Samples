namespace Day03.Core.Tests.AdvancedTheoryTests;

/// <summary>
/// class CalculatorClassDataTests - 用於測試 Calculator 的 ClassData 功能。
/// </summary>
public class CalculatorClassDataTests
{
    [Theory]
    [ClassData(typeof(CalculationTestData))]
    public void Calculate_使用ClassData_應回傳正確結果(double a, double b, double expected, string operation)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var actual = operation switch
        {
            "divide" => calculator.Divide(a, b),
            "multiply" => calculator.Multiply(a, b),
            _ => throw new ArgumentException("Unknown operation")
        };

        // Assert
        Assert.Equal(expected, actual, precision: 2);
    }  
}