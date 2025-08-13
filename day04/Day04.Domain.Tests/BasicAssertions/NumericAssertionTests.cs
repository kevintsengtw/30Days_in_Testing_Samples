using Day04.Domain.Services;

namespace Day04.Domain.Tests.BasicAssertions;

/// <summary>
/// class NumericAssertionTests - 數值斷言測試
/// </summary>
public class NumericAssertionTests
{
    [Theory]
    [InlineData(10, 5, 15)]
    [InlineData(0, -1, 1)]
    [InlineData(-5, -10, 0)]
    public void BeInRange_數值在指定範圍內_應通過驗證(int value, int min, int max)
    {
        value.Should().BeGreaterThan(min)
             .And.BeLessThan(max)
             .And.BeInRange(min, max);
    }

    [Fact]
    public void BeApproximately_浮點數精度比較_應在容許誤差內()
    {
        const double pi = 3.14159;
        const double approximatePi = 3.14;

        // 浮點數精度斷言
        pi.Should().BeApproximately(3.14, 0.01);
        approximatePi.Should().BeApproximately(pi, 0.01);

        // 特殊值斷言
        double.NaN.Should().Be(double.NaN);
        double.PositiveInfinity.Should().Be(double.PositiveInfinity);
    }

    [Fact]
    public void Calculate_加法運算結果_應等於預期值()
    {
        var calculator = new Calculator();

        // 計算結果斷言
        calculator.Add(2, 3).Should().Be(5);
        calculator.Divide(10, 3).Should().BeApproximately(3.33, 0.01);
        calculator.Multiply(0, 100).Should().Be(0);
    }
}