namespace Day04.Domain.Services;

/// <summary>
/// class Calculator - 用於執行數學計算的服務。
/// </summary>
public class Calculator
{
    /// <summary>
    /// 加法
    /// </summary>
    /// <param name="a">第一個數字</param>
    /// <param name="b">第二個數字</param>
    /// <returns>兩個數字的和</returns>
    public int Add(int a, int b)
    {
        return a + b;
    }

    /// <summary>
    /// 除法
    /// </summary>
    /// <param name="a">第一個數字</param>
    /// <param name="b">第二個數字</param>
    /// <returns>兩個數字的商</returns>
    public double Divide(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("除數不能為零");
        }

        return (double)a / b;
    }

    /// <summary>
    /// 乘法
    /// </summary>
    /// <param name="a">第一個數字</param>
    /// <param name="b">第二個數字</param>
    /// <returns>兩個數字的積</returns>
    public int Multiply(int a, int b)
    {
        return a * b;
    }

    /// <summary>
    /// 減法
    /// </summary>
    /// <param name="a">第一個數字</param>
    /// <param name="b">第二個數字</param>
    /// <returns>兩個數字的差</returns>
    public int Subtract(int a, int b)
    {
        return a - b;
    }
}