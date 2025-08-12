namespace Day03.Core.Services;

/// <summary>
/// class Calculator - 用於執行基本數學運算的服務。
/// </summary>
public class Calculator
{
    /// <summary>
    /// 執行加法運算。
    /// </summary>
    /// <param name="a">第一個加數。</param>
    /// <param name="b">第二個加數。</param>
    /// <returns>加法運算的結果。</returns>
    public long Add(int a, int b)
    {
        return (long)a + b;
    }

    /// <summary>
    /// 執行除法運算。
    /// </summary>
    /// <param name="a">被除數。</param>
    /// <param name="b">除數。</param>
    /// <returns>除法運算的結果。</returns>
    public double Divide(double a, double b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Cannot divide by zero");
        }

        return a / b;
    }

    /// <summary>
    /// 執行乘法運算。
    /// </summary>
    /// <param name="a">第一個乘數。</param>
    /// <param name="b">第二個乘數。</param>
    /// <returns>乘法運算的結果。</returns>
    public double Multiply(double a, double b)
    {
        return a * b;
    }
}