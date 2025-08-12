namespace Day03.Core.Tests.AdvancedTheoryTests;

/// <summary>
/// class CsvDataTests - 用於測試 CSV 資料的功能。
/// </summary>
public class CsvDataTests
{
    [Theory]
    [ClassData(typeof(CsvTestData))]
    public void Add_使用CSV資料_應回傳正確結果(int a, int b, int expected)
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var actual = calculator.Add(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }  
}